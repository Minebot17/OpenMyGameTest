using System;
using System.Linq;
using ObservableCollections;
using R3;
using UnityEngine;
using Utils.Extension_Methods;
using VContainer;
using ViewModel.Common;

namespace View.Common
{
    public class SingletonViewBinder : MonoBehaviour
    {
        private enum BindViewType
        {
            BindableView,
            PanelView
        }

        [SerializeField] private Component[] _bindTargets;
        [SerializeField] private BindViewType bindViewType;
        [SerializeField] private bool _bindDynamic;

        private IObjectResolver _container;
        private ILogger _logger;
        private IViewModelsTracker _viewModelsTracker;

        [Inject]
        public void Construct(
            IObjectResolver container,
            IViewModelsTracker viewModelsTracker
        )
        {
            _container = container;
            _viewModelsTracker = viewModelsTracker;
        }

        private void Awake()
        {
            if (_bindDynamic)
            {
                _viewModelsTracker.ViewModels.ObserveAdd()
                    .Subscribe(e =>
                    {
                        if (!_bindDynamic)
                        {
                            return;
                        }

                        FindAndBindViewModelFor(type => e.Value);
                    }).AddTo(this);
                return;
            }

            FindAndBindViewModelFor(type => _container.Resolve(type));
        }

        private void FindAndBindViewModelFor(Func<Type, object> resolver)
        {
            var components = _bindTargets.Length != 0 ? _bindTargets : gameObject.GetComponents<Component>();

            foreach (var component in components)
            {
                var viewType = component.GetType();
                try
                {
                    Type baseDefinition = bindViewType switch
                    {
                        BindViewType.PanelView => typeof(PanelView<>),
                        _ => typeof(BindableView<>)
                    };

                    if (viewType.IsAssignableFromDefinition(baseDefinition, out var generic))
                    {
                        var actualViewModelType = generic[0];

                        // --- МИНИМАЛЬНАЯ ПРАВКА ТУТ ---
                        // Если мы нашли букву "T", идем по иерархии вверх, чтобы найти реальный класс
                        if (actualViewModelType.IsGenericParameter)
                        {
                            var current = viewType;
                            while (current != null && current != typeof(object))
                            {
                                if (current.IsGenericType)
                                {
                                    var args = current.GetGenericArguments();
                                    if (args.Length > 0 && !args[0].IsGenericParameter)
                                    {
                                        actualViewModelType = args[0];
                                        break;
                                    }
                                }
                                current = current.BaseType;
                            }
                        }
                        // ------------------------------

                        var bindToMethod = viewType.GetMethod("BindTo", new[] { actualViewModelType });

                        // Если метод BindTo не найден на самом типе, ищем его в базовых классах
                        if (bindToMethod == null)
                        {
                            bindToMethod = viewType.GetMethods()
                                .FirstOrDefault(m => m.Name == "BindTo" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.IsAssignableFrom(actualViewModelType));
                        }

                        var viewModel = resolver.Invoke(actualViewModelType);

                        if (viewModel == null || !actualViewModelType.IsAssignableFrom(viewModel.GetType()))
                        {
                            continue;
                        }

                        bindToMethod.Invoke(component, new[] { viewModel });
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}