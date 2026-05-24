using System;
using R3;

namespace Utils.Extension_Methods.RX
{
    public interface IPooledDisposable : IDisposable
    {
        Observable<IPooledDisposable> OnDisposed { get; }
        bool IsDisposed { get; }

        void Recover();
    }
}