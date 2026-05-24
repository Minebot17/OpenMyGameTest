using ObservableCollections;

namespace ViewModel.Common
{
    public interface IViewModelsTracker
    {
        ObservableHashSet<TrackedViewModel> ViewModels { get; }
    }
}