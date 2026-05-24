using ObservableCollections;

namespace ViewModel.Common
{
    public class ViewModelsTracker : IViewModelsTracker
    {
        public ObservableHashSet<TrackedViewModel> ViewModels { get; } = new();
    }
}