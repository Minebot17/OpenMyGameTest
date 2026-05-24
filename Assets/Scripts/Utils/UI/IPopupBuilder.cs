using System;

namespace Utils.UI
{
    public interface IPopupBuilder
    {
        IPopupBuilder NewPopup(string name);
        IPopupBuilder AddDescription(string description);
        IPopupBuilder AddOption(string optionName, Action optionAction = null, PopupButtonType style = PopupButtonType.Default);

        IPopupBuilder AddInputField(string placeholder, Action<string> onValueChange = null);

        void Open();
    }
}