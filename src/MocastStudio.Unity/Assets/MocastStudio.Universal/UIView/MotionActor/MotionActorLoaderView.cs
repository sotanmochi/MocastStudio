using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UniVRM10.URPSample;

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
#if UNITY_STANDALONE_WIN
            return FileDialogForWindows.FileDialog("Open VRM", "vrm");
#elif UNITY_EDITOR
            return UnityEditor.EditorUtility.OpenFilePanel("Open VRM", "", "vrm");
#else
            throw new NotImplementedException();
#endif
        }
    }
}
