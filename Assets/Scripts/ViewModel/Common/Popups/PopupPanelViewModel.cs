using ObservableCollections;
using R3;

namespace ViewModel.Common.Popups
{
    public class PopupPanelViewModel : PanelViewModel
    {
        public ReactiveProperty<string> Name { get; } = new();
        public ReactiveProperty<string> Description { get; } = new();

        public ReactiveProperty<string> InputValue { get; } = new("");
        public ReactiveProperty<bool> ShowInput { get; } = new(false);

        public ObservableList<PopupOptionViewModel> PopupOptions { get; } = new();
        private float _openedAtUnscaledTime;

        public PopupPanelViewModel(IViewModelsTracker viewModelsTracker) : base(viewModelsTracker) { }
    }
}
