using System;
using System.Collections.Generic;
using R3;

namespace Utils.Extension_Methods.RX
{
    public sealed class CountingSubject<T> : Observable<T>, IObserver<T>, IDisposable
    {
        public ReactiveCommand<Unit> OnDisposed { get; } = new();
        
        private readonly object _gate = new();
        private readonly T _defaultValue;
        private List<Observer<T>> _observers = new();
        private bool _isStopped;
        private bool _isDisposed;
        private int _subscriberCount;
        private Exception _lastError;

        public CountingSubject(T defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public void OnNext(T value)
        {
            lock (_gate)
            {
                if (_isStopped)
                {
                    return;
                }
                
                foreach (var o in _observers)
                {
                    o.OnNext(value);
                }
            }
        }

        public void OnError(Exception error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            List<Observer<T>> snapshot;
            
            lock (_gate)
            {
                if (_isStopped) return;
                _isStopped = true;
                _lastError = error;
                snapshot = _observers;
                _observers = null;
            }

            foreach (var o in snapshot)
            {
                o.OnErrorResume(error);
            }
            
            Dispose();
        }

        public void OnCompleted()
        {
            List<Observer<T>> snapshot;
            
            lock (_gate)
            {
                if (_isStopped)
                {
                    return;
                }
                
                _isStopped = true;
                snapshot = _observers;
                _observers = null;
            }

            foreach (var o in snapshot)
            {
                o.OnCompleted();
            }
            
            Dispose();
        }
        
        protected override IDisposable SubscribeCore(Observer<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (_gate)
            {
                if (_isDisposed)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                if (_isStopped)
                {
                    if (_lastError != null)
                    {
                        observer.OnErrorResume(_lastError);
                    }
                    else
                    {
                        observer.OnCompleted();
                    }
                    
                    return Disposable.Empty;
                }
                
                _subscriberCount++;
                _observers.Add(observer);
                observer.OnNext(_defaultValue);
            }
            
            return Disposable.Create(() =>
            {
                var needDispose = false;
                
                lock (_gate)
                {
                    if (_observers?.Remove(observer) == true)
                    {
                        _subscriberCount--;
                        
                        if (_subscriberCount == 0 && !_isStopped)
                        {
                            _isStopped = true;
                            needDispose = true;
                        }
                    }
                }

                if (needDispose)
                {
                    Dispose();
                }
            });
        }
        
        public void Dispose()
        {
            lock (_gate)
            {
                if (_isDisposed) return;
                _isDisposed  = true;
                _observers   = null;
            }
            
            OnDisposed.Execute(Unit.Default);
            OnDisposed.Dispose();
        }
    }
}