using System;
using System.Collections.Generic;
using R3;
using Utils.UI;
using VContainer;

namespace ViewModel.Common.Popups
{
    public class PopupBuilder : IPopupBuilder
    {
        private readonly PopupPanelViewModel _popupPanelViewModel;
        private readonly IObjectResolver _container;
        
        private string _name;
        private string _description;
        private readonly List<PopupOptionViewModel> _options = new();
        private string _inputPlaceholder;
        private bool _showInput;

        public PopupBuilder(PopupPanelViewModel popupPanelViewModel, IObjectResolver container)
        {
            _popupPanelViewModel = popupPanelViewModel;
            _container = container;
        }

        public IPopupBuilder NewPopup(string name)
        {
            Clear();
            _name = name;
            return this;
        }

        public IPopupBuilder AddDescription(string description)
        {
            _description = description;
            return this;
        }

        public IPopupBuilder AddOption(string optionName, Action optionAction = null, PopupButtonType style = PopupButtonType.Default)
        {
            var optionVm = _container.Resolve<PopupOptionViewModel>();
            optionVm.Text.Value = optionName;
            optionVm.StyleType.Value = style; // Устанавливаем стиль
            optionVm.Action = () =>
            {
                ClosePanel();
                optionAction?.Invoke();
            };
            _options.Add(optionVm);
            return this;
        }

        public IPopupBuilder AddInputField(string placeholder, Action<string> onValueChange = null)
        {
            _showInput = true;
            _inputPlaceholder = placeholder;

            _popupPanelViewModel.InputValue.Subscribe(onValueChange);

            return this;
        }

        public void Open()
        {
            _popupPanelViewModel.Name.Value = _name;
            _popupPanelViewModel.Description.Value = _description;
            _popupPanelViewModel.PopupOptions.Clear();
            _popupPanelViewModel.ShowInput.Value = _showInput;
            _popupPanelViewModel.InputValue.Value = "";

            foreach (var option in _options)
            {
                _popupPanelViewModel.PopupOptions.Add(option);
            }
            
            _popupPanelViewModel.OpenPanel(Unit.Default);
            Clear();
        }

        private void ClosePanel()
        {
            _popupPanelViewModel.ClosePanel(Unit.Default);
        }

        private void Clear()
        {
            _name = null;
            _description = null;
            _options.Clear();
            _showInput = false;
        }
    }
}