using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Utils.Extension_Methods.RX
{
    public static class ReactiveExtensions
    {
        public static IDisposable SubscribeLevelUp(this ReactiveProperty<int> levelProperty, Action<int> subscriber)
        {
            return levelProperty
                .ThrottleLastFrame(1)
                .Pairwise()
                .Where(pair => pair.Previous < pair.Current)
                .Select(pair => pair.Current)
                .Subscribe(subscriber);
        }

        public static IDisposable DestroyWhenParticlesStopped(this ParticleSystem particleSystem)
        {
            IDisposable disposable = null;
            disposable = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    if (!particleSystem.isPlaying)
                    {
                        Object.Destroy(particleSystem.gameObject);
                        disposable.Dispose();
                    }
                }).AddTo(particleSystem);
            
            return disposable;
        }

        public static ObservableHashSet<TTarget> BindWithWhereAndSelect<TTarget, TSource>(
            this ObservableHashSet<TSource> sourceCollection, 
            Func<TSource, bool> predicate,
            Func<TSource, TTarget> selector,
            DisposableObject.IAdd disposableContainer
        ) {
            var targetCollection = new ObservableHashSet<TTarget>();
            
            sourceCollection.ObserveAdd()
                .Subscribe(e =>
                {
                    if (!predicate(e.Value))
                    {
                        return;
                    }
                    
                    targetCollection.Add(selector.Invoke(e.Value));
                })
                .AddTo(disposableContainer);
            
            sourceCollection.ObserveRemove()
                .Subscribe(e => targetCollection.Remove(selector.Invoke(e.Value)))
                .AddTo(disposableContainer);

            sourceCollection.ObserveClear()
                .Subscribe(e => targetCollection.Clear())
                .AddTo(disposableContainer);

            return targetCollection;
        }

        public static IDisposable AddComponentReactive<T>(this GameObject gameObject, ref T component)
            where T : Component
        {
            if (!gameObject.TryGetComponent(out component))
            {
                component = gameObject.AddComponent<T>();
            }

            var boxedComponent = component;
            return Disposable.Create(() =>
            {
                if (boxedComponent)
                {
                    Object.Destroy(boxedComponent);
                }
            });
        }
        
        public static IDisposable AddChildReactive(this GameObject gameObject, ref GameObject child)
        {
            child = new GameObject("Part");
            child.transform.SetParent(gameObject.transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.zero;

            var boxedChild = child;
            return Disposable.Create(() =>
            {
                if (boxedChild)
                {
                    Object.Destroy(boxedChild);
                }
            });
        }
        
        public static IDisposable InstantiateReactive<T>(
            this IObjectResolver container, 
            ref T newObject, T prefab, Transform parent = null) where T : Component
        {
            newObject = container.Instantiate(prefab);

            if (parent != null)
            {
                newObject.transform.SetParent(parent);
            }

            newObject.transform.localPosition = Vector3.zero;
            newObject.transform.localRotation = Quaternion.identity;
            newObject.transform.localScale = Vector3.one;
            
            var boxedChild = newObject;
            return Disposable.Create(() =>
            {
                if (boxedChild && boxedChild.gameObject)
                {
                    Object.Destroy(boxedChild.gameObject);
                }
            });
        }
        
        public static IDisposable InstantiateReactive(
            this IObjectResolver container, 
            ref GameObject newObject, GameObject prefab, Transform parent = null)
        {
            newObject = container.Instantiate(prefab);

            if (parent != null)
            {
                newObject.transform.SetParent(parent);
            }

            newObject.transform.localPosition = Vector3.zero;
            newObject.transform.localRotation = Quaternion.identity;
            newObject.transform.localScale = Vector3.one;
            
            var boxedChild = newObject;
            return Disposable.Create(() =>
            {
                if (boxedChild)
                {
                    Object.Destroy(boxedChild);
                }
            });
        }
        
        public static ReadOnlyReactiveProperty<TResult> ToAggregatedProperty<TKey,TValue,TResult>(
            this IReadOnlyObservableDictionary<TKey,TValue> src,
            Func<IReadOnlyObservableDictionary<TKey,TValue>, TResult> aggregate)
        {
            return Observable.Merge(
                    src.ObserveDictionaryAdd().AsUnitObservable(),
                    src.ObserveDictionaryRemove().AsUnitObservable(),
                    src.ObserveDictionaryReplace().AsUnitObservable())
                .Select(_ => aggregate(src))
                .Prepend(aggregate(src))
                .DistinctUntilChanged()
                .ToReadOnlyReactiveProperty();
        }
        
        public static Observable<Unit> ObserveAnyChange<TKey,TValue>(this IReadOnlyObservableDictionary<TKey,TValue> src)
        {
            return Observable.Merge(
                    src.ObserveDictionaryAdd().AsUnitObservable(),
                    src.ObserveDictionaryRemove().AsUnitObservable(),
                    src.ObserveDictionaryReplace().AsUnitObservable());
        }
        
        public static IDisposable BindToDictionary<TSource, TTarget, TKey>(
            this ObservableList<TSource> source,
            IDictionary<TKey, TTarget> target,
            Func<TSource, TKey> keySelector,
            Func<TSource, TTarget> valueSelector,
            Predicate<TSource> filter = null
        ) {
            var addDisposable = source.ObserveAdd().Subscribe(e =>
            {
                if (filter == null || filter(e.Value))
                {
                    target.Add(keySelector(e.Value), valueSelector(e.Value));
                }
            });
            var removeDisposable = source.ObserveRemove().Subscribe(
                e => target.Remove(keySelector(e.Value)));
            var clearDisposable = source.ObserveClear().Subscribe(e => target.Clear());
            
            return new CompositeDisposable(addDisposable, removeDisposable, clearDisposable);
        }

        public static IDisposable BindToProperty<T>(this ObservableHashSet<T> set, ReactiveProperty<T> property) where T : class
        {
            var removeDisposable = set.ObserveRemove().Subscribe(e =>
            {
                if (property.Value == e.Value)
                {
                    property.Value = null;
                }
            });
            
            var clearDisposable = set.ObserveClear().Subscribe(e =>
            {
                property.Value = null;
            });

            var replaceDisposable = set.ObserveReplace().Subscribe(e =>
            {
                if (property.Value == e.OldValue)
                {
                    property.Value = e.NewValue;
                }
            });
            
            return new CompositeDisposable(removeDisposable, clearDisposable, replaceDisposable);
        }

        public static IDisposable BindClick(this Button button, ReactiveCommand command)
        {
            return button.OnClickAsObservable().Subscribe(_ => command.Execute(Unit.Default));
        }
        
        public static IDisposable BindClick(this Button button, Action action)
        {
            return button.OnClickAsObservable().Subscribe(_ => action());
        }

        public static IDisposable BindClick(this Button button, Action<Unit> action)
        {
            return button.OnClickAsObservable().Subscribe(_ => action(Unit.Default));
        }

        public static IDisposable BindValueChange(this Toggle toggle, Action<bool> action)
        {
            return toggle.OnValueChangedAsObservable().Subscribe(value => action(value));
        }

        public static IDisposable BindTo<T>(this ReactiveProperty<T> target, ReactiveProperty<T> source)
        {
            return source.Subscribe(e => target.Value = e);
        }
        
        public static IDisposable BindTo<T, N>(this ReactiveProperty<N> target, ReactiveProperty<T> source, Func<T, N> selector)
        {
            return source.Subscribe(e => target.Value = selector(e));
        }
    }
}