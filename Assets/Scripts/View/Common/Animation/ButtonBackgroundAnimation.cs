using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.Common.Animation
{
    public class ButtonBackgroundAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] public Color TargetBackgroundColor;
        [SerializeField] private float _transitionDuration;

        private Color _defaultBackgroundColor;

        private async void Start()
        {
            await UniTask.NextFrame();
            _defaultBackgroundColor = _backgroundImage.color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _backgroundImage.DOColor(TargetBackgroundColor, _transitionDuration);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _backgroundImage.DOColor(_defaultBackgroundColor, _transitionDuration);
        }
    }
}