using R3;
using UnityEngine;
using Utils.Extension_Methods.RX;
using Utils.UI;
using VContainer.Unity;
using ViewModel.Common.Popups;

namespace ViewModel
{
    public class PuzzleSelectionElementViewModel : DisposableObject, IInitializable
    {
        private readonly IPopupBuilder _popupBuilder;
        
        public ReactiveCommand OnSelected { get; set; } = new();

        public ReactiveProperty<string> Id { get; set; } = new();
        public ReactiveProperty<Sprite> Image { get; set; } = new();
        public ReactiveProperty<PuzzleOpenType> OpenType { get; set; } = new();

        public PuzzleSelectionElementViewModel(IPopupBuilder popupBuilder)
        {
            _popupBuilder = popupBuilder;
        }

        public void Initialize()
        {
            OnSelected.Subscribe(OnSelectedClicked).AddTo(this);
        }

        private void OnSelectedClicked(Unit _)
        {
            _popupBuilder
                .NewPopup("Play puzzle")
                .AddDescription(
                    OpenType.Value == PuzzleOpenType.ByAd ? "Watch Ad before play this"
                    : OpenType.Value == PuzzleOpenType.ByCoins ? "Pay coins before play this"
                    : "Free puzzle")
                .AddOption(
                    OpenType.Value == PuzzleOpenType.ByAd ? "Watch Ad"
                    : OpenType.Value == PuzzleOpenType.ByCoins ? "Pay coins"
                    : "Play", OnLevelStarted)
                .Open();
        }

        private void OnLevelStarted()
        {
            // TODO: load selected level
        }
    }
}