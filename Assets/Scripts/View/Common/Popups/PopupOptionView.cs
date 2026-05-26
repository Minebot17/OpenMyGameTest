using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extension_Methods.RX;
using Utils.UI;
using View.Common.Animation;
using ViewModel.Common.Popups;

namespace View.Common.Popups
{
    public class PopupOptionView : BindableView<PopupOptionViewModel>
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _button;

        protected override void OnBind(CompositeDisposable disposables)
        {
            ViewModel.Text.Subscribe(val => _text.text = val).AddTo(disposables);
            _button.BindClick(ViewModel.Action).AddTo(disposables);
        }
    }
}