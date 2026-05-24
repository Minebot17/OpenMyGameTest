using System;
using R3;
using Utils.Extension_Methods.RX;

namespace View.Common
{
    public abstract class BindableView<TViewModel> : BaseBindableView where TViewModel : DisposableObject
    {
        private TViewModel _viewModel;

        protected TViewModel ViewModel => _viewModel;

        public IDisposable BindTo(TViewModel viewModel)
        {
            Disposables.Dispose();
            Disposables = new CompositeDisposable();
            Disposables.AddTo(this);

            _viewModel = viewModel;

            if (viewModel == null)
            {
                return Disposables;
            }

            OnBind(Disposables);

            return Disposables;
        }

        public override IDisposable BindTo(DisposableObject viewModel)
        {
            return BindTo(viewModel as TViewModel);
        }

        public bool IsBindTo(TViewModel viewModel)
        {
            return viewModel == _viewModel;
        }
    }
}
