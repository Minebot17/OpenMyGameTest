using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Utils.Extension_Methods
{
    public static class UniTaskExtensions
    {
        public static async UniTaskVoid SwitchToMainThread(this Action callback)
        {
            await UniTask.SwitchToMainThread();
            callback();
        }
        
        public static void Stop(ref CancellationTokenSource cts)
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }

        public static UniTask AddToBuffer(this UniTask task, List<UniTask> list)
        {
            list.Add(task);
            return task;
        }
    }
}