using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using ViewModel;

namespace Config
{
    [CreateAssetMenu(menuName = "Settings/PuzzlesMockSettings")]
    public class PuzzlesMockSettings : ScriptableObject, IPuzzleSelectionElementsProvider
    {
        [field: SerializeField] public List<PuzzleMock> PuzzleMocks { get; private set; }
        
        public List<PuzzleSelectionElementViewModel> GetPuzzles(IObjectResolver objectResolver)
        {
            return PuzzleMocks.Select(puzzle =>
            {
                var elementVm = objectResolver.Resolve<PuzzleSelectionElementViewModel>();
                elementVm.Id.Value = puzzle.Id;
                elementVm.Image.Value = puzzle.Image;
                elementVm.OpenType.Value = puzzle.OpenType;
                elementVm.Initialize();
                return elementVm;
            }).ToList();
        }
    }
}