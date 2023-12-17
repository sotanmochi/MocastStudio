using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using SFB;

namespace MocastStudio.Universal.UIView.MotionActor
{
    public sealed class MotionActorLoaderView : MonoBehaviour
    {
        [SerializeField] Button _button;

        private readonly Subject<MotionActorLoadingParameters> _loadingSubject = new();

        public IObservable<MotionActorLoadingParameters> OnLoadingRequested => _loadingSubject;

        void Awake() => Initialize();

        private void Initialize()
        {
            _button.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(async _ =>
                {
                    try
                    {
                        var resourcePath = await GetResourcePathAsync();
                        if (string.IsNullOrEmpty(resourcePath)) return;
                        _loadingSubject.OnNext(new MotionActorLoadingParameters(resourcePath));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                });
        }

        private async Task<string> GetResourcePathAsync()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            var resourcePath = "";
            var taskCompletionSource = new TaskCompletionSource<string>();

            StandaloneFileBrowser.OpenFilePanelAsync("Open VRM", "", "vrm", false, paths =>
            {
                var path = paths.Length > 0 ? paths[0] : "";
                taskCompletionSource.SetResult(path);
            });
 
            resourcePath = await taskCompletionSource.Task;
            return resourcePath;
#else
            throw new NotImplementedException();
#endif
        }
    }
}
