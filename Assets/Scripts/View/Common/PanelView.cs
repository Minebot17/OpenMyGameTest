using System;
using System.Threading.Tasks;
using R3;
using UnityEngine;
using Utils;
using VContainer;
using View.Common.Transitions;
using ViewModel.Common;

namespace View.Common
{
    public abstract class PanelView<TViewModel> : BindableView<TViewModel> where TViewModel : PanelViewModel
    {
        protected abstract Type TransitionType { get; }

        protected IPanelTransition _transition;

        [Inject]
        public void ConstructPanelView(IObjectResolver objectResolver)
        {
            if(TransitionType == null)
            {
                return;
            }

            _transition = objectResolver.Resolve(TransitionType) as IPanelTransition;
            
            if (_transition is IInitializable<GameObject> initializable)
            {
                initializable.Initialize(gameObject);
            }
        }

        protected override void OnBind(CompositeDisposable disposables)
        {
            ViewModel.IsOpened.Subscribe(OnOpenedChanged).AddTo(disposables);
            OnBindInternal(disposables);
        }

        protected abstract void OnBindInternal(CompositeDisposable disposables);

        private async void OnOpenedChanged(bool isOpened)
        {
            if (!this)
            {
                Disposables?.Dispose();
                return;
            }
            
            if (isOpened && !gameObject.activeSelf)
            {
                await OnOpen();
            }
            else if (!isOpened && gameObject.activeSelf)
            {
                await OnClose();
            }
        }

        protected virtual async Task OnOpen()
        {
            gameObject.SetActive(true);

            if(_transition == null)
            {
                return;
            }

            await _transition.FadeIn(ViewModel.TransitionDuration.Value);
        }

        protected virtual async Task OnClose()
        {
            if (_transition != null)
            {
                await _transition.FadeOut(ViewModel.TransitionDuration.Value);
            }
            
            if (!this)
            {
                return;
            }

            // OnClose can overlap with a new OpenPanel() call.
            // In that case we must reopen instead of forcing deactivation.
            if (ViewModel.IsOpened.Value)
            {
                await OnOpen();
                return;
            }
            
            if (gameObject != null) //TODO возможно утечка подписок
            {
                gameObject.SetActive(false);
            }
        }
    }
}
