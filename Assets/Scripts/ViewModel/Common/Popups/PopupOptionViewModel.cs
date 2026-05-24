using System;
using R3;
using Utils.Extension_Methods.RX;
using Utils.UI;

namespace ViewModel.Common.Popups
{
    public class PopupOptionViewModel : DisposableObject
    {
        public ReactiveProperty<string> Text { get; } = new();
        public Action Action { get; set; }

        public ReactiveProperty<PopupButtonType> StyleType { get; } = new(PopupButtonType.Default);
    }
}