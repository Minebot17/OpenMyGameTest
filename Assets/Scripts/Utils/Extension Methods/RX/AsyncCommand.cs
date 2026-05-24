using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;

namespace Utils.Extension_Methods.RX
{
    public class AsyncCommand
    {
        private readonly List<Func<UniTask>> _observers = new ();
        
        public async UniTask Execute()
        {
            foreach (var observer in _observers)
            {
                await observer.Invoke();
            }
        }

        public IDisposable Subscribe(Func<UniTask> observer)
        {
            _observers.Add(observer);
            return Disposable.Create(() => _observers.Remove(observer));
        }
    }
    
    public class AsyncCommand<T>
    {
        private readonly List<Func<T, UniTask>> _observers = new ();
        
        public async UniTask Execute(T data)
        {
            foreach (var observer in _observers)
            {
                await observer.Invoke(data);
            }
        }

        public IDisposable Subscribe(Func<T, UniTask> observer)
        {
            _observers.Add(observer);
            return Disposable.Create(() => _observers.Remove(observer));
        }
    }
    
    public class AsyncCommand<T, N>
    {
        private readonly List<Func<T, N, UniTask>> _observers = new ();
        
        public async UniTask Execute(T data1, N data2)
        {
            foreach (var observer in _observers)
            {
                await observer.Invoke(data1, data2);
            }
        }

        public IDisposable Subscribe(Func<T, N, UniTask> observer)
        {
            _observers.Add(observer);
            return Disposable.Create(() => _observers.Remove(observer));
        }
    }
}