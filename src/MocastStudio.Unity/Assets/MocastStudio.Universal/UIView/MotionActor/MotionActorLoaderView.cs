using System;
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
                .Subscribe(_ =>
                {
                    try
                    {
                        var resourcePath = GetResourcePath();
                        if (string.IsNullOrEmpty(resourcePath)) return;
                        _loadingSubject.OnNext(new MotionActorLoadingParameters(resourcePath));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                });
        }

        private string GetResourcePath()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            var resourcePath = "";

            StandaloneFileBrowser.OpenFilePanelAsync("Open VRM", "", "vrm", false, paths =>
                resourcePath = paths.Length > 0 ? paths[0] : "");

            return resourcePath;
#else
            throw new NotImplementedException();
#endif
        }
    }
}
