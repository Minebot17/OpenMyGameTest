using System.Collections.Generic;
using VContainer;

namespace ViewModel
{
    public interface IPuzzleSelectionElementsProvider
    {
        List<PuzzleSelectionElementViewModel> GetPuzzles(IObjectResolver objectResolver);
    }
}