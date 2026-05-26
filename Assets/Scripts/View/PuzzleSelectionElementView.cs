using R3;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extension_Methods.RX;
using View.Common;
using ViewModel;

namespace View
{
    public class PuzzleSelectionElementView : BindableView<PuzzleSelectionElementViewModel>
    {
        [SerializeField] private Button _selectButton;
        [SerializeField] private Image _puzzlePreviewImage;
        
        protected override void OnBind(CompositeDisposable disposables)
        {
            ViewModel.Image.Subscribe(sprite => _puzzlePreviewImage.sprite = sprite).AddTo(disposables);
            _selectButton.BindClick(ViewModel.OnSelected).AddTo(disposables);
        }
    }
}