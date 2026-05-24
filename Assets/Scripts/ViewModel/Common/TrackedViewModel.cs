using Cysharp.Threading.Tasks;
using Utils.Extension_Methods.RX;
using VContainer.Unity;

namespace ViewModel.Common
{
    public abstract class TrackedViewModel : DisposableObject, IStartable
    {
        private readonly IViewModelsTracker _viewModelsTracker;

        protected TrackedViewModel(IViewModelsTracker viewModelsTracker)
        {
            _viewModelsTracker = viewModelsTracker;
        }

        public virtual async void Start()
        {
            await UniTask.NextFrame();
            _viewModelsTracker.ViewModels.Add(this);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            _viewModelsTracker.ViewModels.Remove(this);
        }
    }
}