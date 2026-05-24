using R3;

namespace ViewModel.Common
{
    public abstract class PanelViewModel : TrackedViewModel
    {
        public const float DefaultTransitionDuration = 0.2f;

        protected PanelViewModel(IViewModelsTracker viewModelsTracker) : base(viewModelsTracker)
        {
        }

        public virtual ReactiveProperty<float> TransitionDuration { get; } = new(DefaultTransitionDuration);
        public virtual ReactiveProperty<bool> IsOpened { get; } = new();
        public virtual PanelViewModel PreviousPanel { get; set; }

        public virtual void OpenPanel(Unit _)
        {
            IsOpened.Value = true;
        }

        public virtual void ClosePanel(Unit _)
        {
            IsOpened.Value = false;
        }
    }
}