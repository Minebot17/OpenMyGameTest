using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.Common.Animation
{
    public class OptionButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _pointImage;
        [SerializeField] private TMP_Text _optionText;
        [SerializeField] private RectTransform _optionTransform;
        [SerializeField] private Color _targetTextColor;
        [SerializeField] private float _textOffsetValue;
        [SerializeField] private float _transitionDuration;

        private Color _defaultTextColor;
        private Vector2 _spawnAnchoredPosition;
        private bool _enableColorChanging = true;

        private async void Start()
        {
            await UniTask.NextFrame();
            _defaultTextColor = _optionText.color;
            _spawnAnchoredPosition = _optionTransform.anchoredPosition;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _optionTransform.DOAnchorPosX(_spawnAnchoredPosition.x + _textOffsetValue, _transitionDuration);
            
            if (_enableColorChanging)
            {
                _optionText.DOColor(_targetTextColor, _transitionDuration);
            }

            _pointImage.DOFade(1f, _transitionDuration);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _optionTransform.DOAnchorPosX(_spawnAnchoredPosition.x, _transitionDuration);

            if (_enableColorChanging)
            {
                _optionText.DOColor(_defaultTextColor, _transitionDuration);
            }

            _pointImage.DOFade(0f, _transitionDuration);
        }

        public void ToggleColorChanging(bool enable)
        {
            _enableColorChanging = enable;
        }
    }
}