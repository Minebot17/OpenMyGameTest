using System;
using R3;
using UnityEngine;
using Utils.Extension_Methods.RX;

namespace View.Common
{
    public abstract class BaseBindableView : MonoBehaviour
    {
        protected CompositeDisposable Disposables = new();

        public abstract IDisposable BindTo(DisposableObject viewModel);

        protected abstract void OnBind(CompositeDisposable disposables);
    }
}