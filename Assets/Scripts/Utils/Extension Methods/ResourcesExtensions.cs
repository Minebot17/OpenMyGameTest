using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine.Networking;
#endif

namespace Utils.Extension_Methods
{
    public static class ResourcesExtensions
    {
        public static async void LoadStreamingAsset(this string filename, Action<string> onSuccess, Action<string> onError)
        {
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, filename);

#if UNITY_WEBGL && !UNITY_EDITOR
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                await www.SendWebRequest().ToUniTask();

                if (www.result != UnityWebRequest.Result.Success)
                    onError?.Invoke(www.error);
                else
                    onSuccess?.Invoke(www.downloadHandler.text);
            }
#else
            if (File.Exists(path))
                onSuccess?.Invoke(File.ReadAllText(path));
            else
                onError?.Invoke("File not found: " + path);
#endif
        }

        public static async UniTask<string> LoadStreamingAsset(this string filename)
        {
            var completionSource = new UniTaskCompletionSource<string>();

            filename.LoadStreamingAsset(content =>
            {
                completionSource.TrySetResult(content);
            }, error =>
            {
                completionSource.TrySetResult(error);
            });

            return await completionSource.Task;
        }
    }
}