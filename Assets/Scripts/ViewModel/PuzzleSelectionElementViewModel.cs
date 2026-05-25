using R3;
using UnityEngine;
using Utils.Extension_Methods.RX;

namespace ViewModel
{
    public class PuzzleSelectionElementViewModel : DisposableObject
    {
        public ReactiveCommand OnSelected { get; set; } = new();

        public ReactiveProperty<string> PuzzleId { get; set; } = new();
        public ReactiveProperty<Sprite> PuzzleImage { get; set; } = new();
    }
}