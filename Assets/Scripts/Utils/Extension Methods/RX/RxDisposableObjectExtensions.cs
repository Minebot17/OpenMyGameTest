using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Utils.Extension_Methods.RX
{
    public class DisposableObject : IPooledDisposable, DisposableObject.IAdd
    {
        private readonly ReactiveCommand<IPooledDisposable> _onDisposed = new();
        private CompositeDisposable _disposables = new();

        [JsonIgnore] public Observable<IPooledDisposable> OnDisposed => _onDisposed;
        [JsonIgnore] public bool IsDisposed { get; private set; }

        public virtual void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            _onDisposed.Execute(this);

            _disposables.Dispose();
        }

        void IAdd.Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public void Recover()
        {
            _disposables = new CompositeDisposable();
            IsDisposed = false;
        }

        public interface IAdd
        {
            public void Add(IDisposable disposable);
        }
    }

    public static class RxDisposableObjectExtensions
    {
        public static T AddTo<T>(this T disposable, DisposableObject.IAdd disposableObject) where T : IDisposable
        {
            disposableObject.Add(disposable);
            return disposable;
        }

        public static T AddTo<T, N>(this T disposable, IObservableCollection<N> collection) where T : class, IDisposable where N : class
        {
            collection.ObserveRemove().Subscribe(x =>
            {
                if (x.Value == disposable)
                {
                    disposable.Dispose();
                }
            });

            collection.ObserveReplace().Subscribe(x =>
            {
                if (x.OldValue == disposable)
                {
                    disposable.Dispose();
                }
            });

            collection.ObserveClear().Subscribe(x =>
            {
                disposable.Dispose();
            });

            return disposable;
        }

        public static T AddTo<T, N>(
            this T disposable, UniTaskCompletionSource<N> completionSource) where T : IDisposable
        {
            completionSource.OnCompleted(_ => disposable.Dispose(), null, 0);
            return disposable;
        }

        public static T AddTo<T>(
            this T disposable, UniTaskCompletionSource completionSource) where T : IDisposable
        {
            completionSource.OnCompleted(_ => disposable.Dispose(), null, 0);
            return disposable;
        }

        public static IDisposable CreateDestroyDisposable(this GameObject gameObject)
        {
            return Disposable.Create(gameObject, obj =>
            {
                if (obj != null)
                    UnityEngine.Object.Destroy(obj);
            });
        }

        public static CancellationTokenSource AddTo(
            this CancellationTokenSource tokenSource, DisposableObject.IAdd disposableObject)
        {
            disposableObject.Add(Disposable.Create(() =>
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }));
            return tokenSource;
        }

        public static UniTask AddTo(
            this UniTask task, DisposableObject disposableObject)
        {
            var tokenSource = new CancellationTokenSource();
            var newTask = task.ContinueWith(() =>
            {
                if (!disposableObject.IsDisposed)
                {
                    disposableObject.Dispose();
                }
            }).AttachExternalCancellation(tokenSource.Token);

            tokenSource.AddTo(disposableObject);
            return newTask;
        }

        public static UniTask AddTo(
            this UniTask task, CompositeDisposable disposableObject)
        {
            var tokenSource = new CancellationTokenSource();
            var newTask = task.AttachExternalCancellation(tokenSource.Token);
            tokenSource.AddTo(disposableObject);
            return newTask;
        }

        public static T AddTo<T, N>(this T disposable, Observable<N> disposableObject) where T : IDisposable
        {
            IDisposable subscription = null;
            subscription = disposableObject.Subscribe(_ =>
            {
                subscription.Dispose();
                disposable.Dispose();
            });
            return disposable;
        }

        public static IDisposable Bind<TSource, TTarget, TKey>(
            this ObservableDictionary<TKey, TSource> target,
            IDictionary<TKey, TTarget> bindable,
            Func<TKey, TSource, TTarget> selection,
            Action<(TKey key, TSource source, TTarget target)> onAddedCallback = null,
            bool triggerExistingElements = false
        )
        {
            var addDisposable = target.ObserveDictionaryAdd().Subscribe(e =>
            {
                var newView = selection.Invoke(e.Key, e.Value);
                onAddedCallback?.Invoke((e.Key, e.Value, newView));
                bindable.Add(e.Key, newView);
            });

            var removeDisposable = target.ObserveDictionaryRemove().Subscribe(e =>
            {
                bindable.Remove(e.Key);
            });

            var replaceDisposable = target.ObserveDictionaryReplace().Subscribe(e =>
            {
                if (bindable.TryGetValue(e.Key, out var toRemove))
                {
                    var newView = selection.Invoke(e.Key, e.NewValue);
                    bindable[e.Key] = newView;
                }
            });

            var clearDisposable = target.ObserveClear().Subscribe(e =>
            {
                bindable.Clear();
            });

            if (triggerExistingElements)
            {
                foreach (var pair in target)
                {
                    var newView = selection.Invoke(pair.Key, pair.Value);
                    onAddedCallback?.Invoke((pair.Key, pair.Value, newView));
                    bindable.Add(pair.Key, newView);
                }
            }

            var result = new CompositeDisposable();
            result.Add(addDisposable);
            result.Add(removeDisposable);
            result.Add(replaceDisposable);
            result.Add(clearDisposable);
            return result;
        }
    }
}