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
        [SerializeField] private TMP_Text _keyText;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;
        [SerializeField] private ButtonBackgroundAnimation _backgroundAnimation;
        [SerializeField] private Button _button;

        protected override void OnBind(CompositeDisposable disposables)
        {
            ViewModel.Text.Subscribe(val => _text.text = val).AddTo(disposables);
            _button.BindClick(ViewModel.Action).AddTo(disposables);

            ViewModel.StyleType.Subscribe(HandleStyle).AddTo(disposables);
        }

        private void HandleStyle(PopupButtonType style)
        {
            switch (style)
            {
                case PopupButtonType.Default:
                    _background.color = new Color32(0, 0, 0, 0);
                    _keyText.transform.parent.gameObject.SetActive(false);
                    _icon.gameObject.SetActive(false);
                    break;
                case PopupButtonType.Confirm:
                    _background.color = new Color32(59, 154, 52, 255);
                    _backgroundAnimation.TargetBackgroundColor = _background.color - new Color32(0, 0, 0, 64);
                    _keyText.text = "y";
                    _keyText.transform.parent.gameObject.SetActive(true);
                    _icon.gameObject.SetActive(true);
                    _icon.transform.rotation = Quaternion.identity;
                    break;
                case PopupButtonType.Cancel:
                    _background.color = new Color32(161, 31, 38, 255);
                    _backgroundAnimation.TargetBackgroundColor = _background.color - new Color32(0, 0, 0, 64);
                    _keyText.text = "n";
                    _keyText.transform.parent.gameObject.SetActive(true);
                    _icon.gameObject.SetActive(true);
                    _icon.transform.rotation = Quaternion.Euler(180, 0, 0);
                    break;
                default:
                    break;
            }
        }

    }
}