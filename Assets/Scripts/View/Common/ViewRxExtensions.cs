using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using UnityEngine;
using Utils.Extension_Methods.RX;
using VContainer;
using VContainer.Unity;

namespace View.Common
{
    public static class ViewRxExtensions
    {
        public static IDisposable BindView<TViewModel, TView>(
            this ReactiveProperty<TViewModel> property, 
            TView view, 
            bool disableOnNull = false
        ) 
            where TViewModel : DisposableObject
            where TView : BindableView<TViewModel>
        {
            return property.Subscribe(vm =>
            {
                if (disableOnNull)
                {
                    view.gameObject.SetActive(vm != null);
                }
                
                if (vm == null)
                {
                    return;
                }
                
                view.BindTo(vm);
            });
        }

        public static IDisposable BindViews<TSource, TTarget>(
            this ObservableList<TSource> target,
            IList<TTarget> bindable,
            IObjectResolver container,
            TTarget viewPrefab,
            Transform viewParent,
            Action<(TSource viewModel, TTarget view)> onAddedCallback = null,
            bool triggerExistingElements = false
        ) 
            where TTarget : BindableView<TSource>
            where TSource : DisposableObject
        {
            var addDisposable = target.ObserveAdd().Subscribe(e =>
            {
                var newView = container.Instantiate(viewPrefab, viewParent);
                onAddedCallback?.Invoke((e.Value, newView));
                newView.BindTo(e.Value);
                bindable.Add(newView);
            });

            var removeDisposable = target.ObserveRemove().Subscribe(e =>
            {
                var toRemove = bindable.FirstOrDefault(x => x.IsBindTo(e.Value));

                if (toRemove != null)
                {
                    bindable.Remove(toRemove);
                    UnityEngine.Object.Destroy(toRemove.gameObject);
                }
            });

            var clearDisposable = target.ObserveClear().Subscribe(e =>
            {
                foreach (var view in bindable)
                {
                    UnityEngine.Object.Destroy(view.gameObject);
                }
                
                bindable.Clear();
            });

            if (triggerExistingElements)
            {
                foreach (var value in target)
                {
                    var newView = container.Instantiate(viewPrefab, viewParent);
                    onAddedCallback?.Invoke((value, newView));
                    newView.BindTo(value);
                    bindable.Add(newView);
                }
            }

            var result = new CompositeDisposable();
            result.Add(addDisposable);
            result.Add(removeDisposable);
            result.Add(clearDisposable);
            return result;
        }
        
        public static IDisposable BindViews<TSource, TTarget, TKey>(
            this ObservableDictionary<TKey, TSource> target,
            IDictionary<TKey, TTarget> bindable,
            IObjectResolver container,
            TTarget viewPrefab,
            Transform viewParent,
            Action<(TKey key, TSource viewModel, TTarget view)> onAddedCallback = null,
            bool triggerExistingElements = false
        ) 
            where TTarget : BindableView<TSource>
            where TSource : DisposableObject
        {
            var addDisposable = target.ObserveDictionaryAdd().Subscribe(e =>
            {
                var newView = container.Instantiate(viewPrefab, viewParent);
                onAddedCallback?.Invoke((e.Key, e.Value, newView));
                newView.BindTo(e.Value);
                bindable.Add(e.Key, newView);
            });

            var removeDisposable = target.ObserveDictionaryRemove().Subscribe(e =>
            {
                if (bindable.Remove(e.Key, out var toRemove))
                {
                    UnityEngine.Object.Destroy(toRemove.gameObject);
                }
            });
            
            var replaceDisposable = target.ObserveDictionaryReplace().Subscribe(e =>
            {
                if (bindable.TryGetValue(e.Key, out var toRemove))
                {
                    UnityEngine.Object.Destroy(toRemove.gameObject);
                    var newView = container.Instantiate(viewPrefab, viewParent);
                    newView.BindTo(e.NewValue);
                    bindable[e.Key] = newView;
                }
            });

            var clearDisposable = target.ObserveClear().Subscribe(e =>
            {
                foreach (var pair in bindable)
                {
                    UnityEngine.Object.Destroy(pair.Value.gameObject);
                }
                
                bindable.Clear();
            });

            if (triggerExistingElements)
            {
                foreach (var pair in target)
                {
                    var newView = container.Instantiate(viewPrefab, viewParent);
                    onAddedCallback?.Invoke((pair.Key, pair.Value, newView));
                    newView.BindTo(pair.Value);
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