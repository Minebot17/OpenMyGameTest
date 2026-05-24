using System;
using System.Collections.Generic;
using R3;
using TMPro;
using UnityEngine;
using VContainer;
using View.Common.Transitions;
using ViewModel.Common.Popups;

namespace View.Common.Popups
{
    public class PopupPanelView : PanelView<PopupPanelViewModel>
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private PopupOptionView _popupOptionPrefab;
        [SerializeField] private Transform _popupOptionContainer;

        private IObjectResolver _container;

        private readonly List<PopupOptionView> _optionViews = new();

        protected override Type TransitionType => typeof(AlphaPanelTransition);

        [Inject]
        public void Construct(IObjectResolver container)
        {
            _container = container;
        }

        protected override void OnBindInternal(CompositeDisposable disposables)
        {
            ViewModel.Name.Subscribe(n => _nameText.text = n).AddTo(disposables);
            ViewModel.Description.Subscribe(d => _descriptionText.text = d).AddTo(disposables);
            ViewModel.PopupOptions.BindViews(_optionViews, _container, _popupOptionPrefab, _popupOptionContainer).AddTo(disposables);

            _inputField.text = ViewModel.InputValue.Value;
            _inputField.onValueChanged.AsObservable().Subscribe(d =>
            {
                ViewModel.InputValue.Value = d;
            }).AddTo(disposables);

            ViewModel.ShowInput.Subscribe(d =>
            {
                //TODO возможно утечка подписок
                if (_inputField != null)
                {
                    _inputField.gameObject.SetActive(d);
                }
            }).AddTo(disposables);
        }
    }
}