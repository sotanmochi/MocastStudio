using UnityEngine;
using UnityEngine.UI;

namespace MocastStudio.Presentation.CameraSystem
{
    public sealed class CameraOutputView : MonoBehaviour
    {
        [SerializeField] Canvas _mainCameraCanvas;
        [SerializeField] RawImage _mainCameraImage;

        [SerializeField] Canvas _switcherCanvas;
        [SerializeField] RawImage _switcherMain;
        [SerializeField] RawImage _switcherPreview;
        [SerializeField] RawImage _switcherCamera1;
        [SerializeField] RawImage _switcherCamera2;
        [SerializeField] RawImage _switcherCamera3;
        [SerializeField] RawImage _switcherCamera4;

        bool _initialized;

        RenderTexture _mainRenderTexture;
        RenderTexture _previewRenderTexture;
        RenderTexture _cameraRenderTexture1;
        RenderTexture _cameraRenderTexture2;
        RenderTexture _cameraRenderTexture3;
        RenderTexture _cameraRenderTexture4;

        public RenderTexture MainRenderTexture => _mainRenderTexture;
        public RenderTexture PreviewRenderTexture => _previewRenderTexture;
        public RenderTexture CameraRenderTexture1 => _cameraRenderTexture1;
        public RenderTexture CameraRenderTexture2 => _cameraRenderTexture2;
        public RenderTexture CameraRenderTexture3 => _cameraRenderTexture3;
        public RenderTexture CameraRenderTexture4 => _cameraRenderTexture4;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) return;

            _mainRenderTexture = new RenderTexture(1920, 1080, 0);
            _previewRenderTexture = new RenderTexture(960, 540, 0);
            _cameraRenderTexture1 = new RenderTexture(480, 270, 0);
            _cameraRenderTexture2 = new RenderTexture(480, 270, 0);
            _cameraRenderTexture3 = new RenderTexture(480, 270, 0);
            _cameraRenderTexture4 = new RenderTexture(480, 270, 0);

            _mainCameraImage.texture = _mainRenderTexture;
            _switcherMain.texture = _mainRenderTexture;
            _switcherPreview.texture = _previewRenderTexture;
            _switcherCamera1.texture = _cameraRenderTexture1;
            _switcherCamera2.texture = _cameraRenderTexture2;
            _switcherCamera3.texture = _cameraRenderTexture3;
            _switcherCamera4.texture = _cameraRenderTexture4;

            _initialized = true;
        }

        public void ShowMainCamera()
        {
            _mainCameraCanvas.enabled = true;
            _switcherCanvas.enabled = false;
        }

        public void ShowSwitcherView()
        {
            _mainCameraCanvas.enabled = false;
            _switcherCanvas.enabled = true;
        }

        public void Hide()
        {
            _mainCameraCanvas.enabled = false;
            _switcherCanvas.enabled = false;
        }
    }
}
