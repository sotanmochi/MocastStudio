using System.Collections.Generic;
using UnityEngine;

namespace MocastStudio.Presentation.CameraSystem
{
    public sealed class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] SimpleCameraController _simpleCameraController;
        [SerializeField] CameraOutputView _outputView;
        [SerializeField] Camera _outputViewCamera;
        [SerializeField] Camera _sceneViewCamera;
        [SerializeField] List<Camera> _cameras = new();

        bool _initialized;
        Camera _currentMainCamera;

        public bool Initialized => _initialized;
        public Texture MainRenderTexture => _outputView.MainRenderTexture;

        void Start() => Initialize();

        public void Initialize()
        {
            if (_initialized) return;

            _outputView.Initialize();

            for (var i = 0; i < _cameras.Count; i++)
            {
                var camera = _cameras[i];
                var targetTexture = i switch
                {
                    0 => _outputView.CameraRenderTexture1,
                    1 => _outputView.CameraRenderTexture2,
                    2 => _outputView.CameraRenderTexture3,
                    3 => _outputView.CameraRenderTexture4,
                };
                camera.targetTexture = targetTexture;
            }

            _cameras[0].targetTexture = _outputView.MainRenderTexture;
            _cameras[1].targetTexture = _outputView.PreviewRenderTexture;

            _currentMainCamera = _cameras[0];

            _initialized = true;
        }

        public void SwitchToMainCamera()
        {
            _simpleCameraController.SetCamera(_currentMainCamera);

            _sceneViewCamera.enabled = false;
            _outputViewCamera.enabled = true;
            _outputView.ShowMainCamera();

            _currentMainCamera.enabled = true;
            foreach (var camera in _cameras)
            {
                if (camera == _currentMainCamera) continue;
                camera.enabled = false;
            }
        }

        public void SwitchToSceneCamera()
        {
            _simpleCameraController.SetCamera(_sceneViewCamera);

            _sceneViewCamera.enabled = true;
            _outputViewCamera.enabled = false;
            _outputView.Hide();

            // NOTE: Spout/Syphonで映像を送信するためにMainCameraは有効にしておく
            _currentMainCamera.enabled = true;
            foreach (var camera in _cameras)
            {
                if (camera == _currentMainCamera) continue;
                camera.enabled = false;
            }
        }

        public void SwitchToSwitcherView()
        {
            _simpleCameraController.SetCamera(null);

            _sceneViewCamera.enabled = false;
            _outputViewCamera.enabled = true;
            _outputView.ShowSwitcherView();

            foreach (var camera in _cameras)
            {
                camera.enabled = true;
            }
        }
    }
}
