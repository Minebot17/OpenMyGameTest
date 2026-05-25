using ObservableCollections;
using VContainer;
using ViewModel.Common;

namespace ViewModel
{
    public class PuzzleSelectionPanelViewModel : PanelViewModel
    {
        private readonly IPuzzleSelectionElementsProvider _puzzleSelectionElementsProvider;
        private readonly IObjectResolver _objectResolver;
        
        public ObservableDictionary<string, PuzzleSelectionElementViewModel> PuzzleSelectionElements { get; } = new();

        public PuzzleSelectionPanelViewModel(
            IViewModelsTracker viewModelsTracker, 
            IPuzzleSelectionElementsProvider puzzleSelectionElementsProvider,
            IObjectResolver objectResolver
            ) : base(viewModelsTracker)
        {
            _puzzleSelectionElementsProvider = puzzleSelectionElementsProvider;
            _objectResolver = objectResolver;
        }

        public override void Start()
        {
            base.Start();

            IsOpened.Value = true;
            
            foreach (var puzzleVm 
                     in _puzzleSelectionElementsProvider.GetPuzzles(_objectResolver))
            {
                PuzzleSelectionElements.Add(puzzleVm.PuzzleId.Value, puzzleVm);
            }
        }
    }
}