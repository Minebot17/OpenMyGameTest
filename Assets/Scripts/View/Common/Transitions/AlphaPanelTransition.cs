using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace View.Common.Transitions
{
    public class AlphaPanelTransition : IPanelTransition, IInitializable<GameObject>
    {
        private CanvasGroup _canvasGroup;
        
        public void Initialize(GameObject panelObject)
        {
            _canvasGroup = panelObject.GetComponent<CanvasGroup>();
        }
        
        public async UniTask FadeIn(float duration)
        {
            _canvasGroup.interactable = false;
            await _canvasGroup.DOFade(1f, duration);

            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
        }

        public async UniTask FadeOut(float duration)
        {
            if (_canvasGroup.alpha == 0)
            {
                return;
            }

            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            await _canvasGroup.DOFade(0f, duration);
        }
    }
}