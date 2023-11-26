using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UniVRM10.URPSample;

namespace MocastStudio.Samples.Receiver.UIView
{
    public sealed class MotionActorLoaderUIView : MonoBehaviour
    {
        [SerializeField] Button _button;

        private readonly Subject<string> _loadingRequested = new();

        public IObservable<string> OnLoadingRequested => _loadingRequested;

        void Awake() => Initialize();

        void Initialize()
        {
            _button.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => 
                {
#if UNITY_STANDALONE_WIN
                    var resourcePath = FileDialogForWindows.FileDialog("Open VRM", "vrm");
#elif UNITY_EDITOR
                    var resourcePath = UnityEditor.EditorUtility.OpenFilePanel("Open VRM", "", "vrm");
#else
                    throw new NotImplementedException();
#endif
                    if (string.IsNullOrEmpty(resourcePath))
                    {
                        return;
                    }

                    _loadingRequested.OnNext(resourcePath);
                });
        }
    }
}
