using Config;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View.Common.Transitions;
using ViewModel;
using ViewModel.Common;
using ViewModel.Common.Popups;

namespace DI
{
    public class MainMenuScope : LifetimeScope
    {
        [SerializeField] private PuzzlesMockSettings _puzzlesMockSettings; 
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<AlphaPanelTransition>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InstantPanelTransition>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<ViewModelsTracker>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<PuzzleSelectionPanelViewModel>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PopupPanelViewModel>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<PuzzleSelectionElementViewModel>(Lifetime.Transient).AsImplementedInterfaces().AsSelf();
            
            builder.RegisterInstance(_puzzlesMockSettings).AsImplementedInterfaces();
        }
    }
}