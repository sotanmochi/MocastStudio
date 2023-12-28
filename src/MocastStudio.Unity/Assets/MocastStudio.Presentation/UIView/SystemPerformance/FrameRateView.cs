using UnityEngine;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Universal.UIView.SystemPerformance
{
    public sealed class FrameRateView : MonoBehaviour
    {
        [SerializeField] Text _fpsText;

        private float _previousFrameTime;
        private float _averageFps;

        void Update()
        {
            var currentFrameTime = Time.realtimeSinceStartup;
            var deltaTime = currentFrameTime - _previousFrameTime;
            _previousFrameTime = currentFrameTime;

            var fps = 1f / deltaTime;

            _averageFps += fps;
            if (Time.frameCount % 10 == 0)
            {
                _averageFps /= 10;
                _fpsText.text = $"FPS: {_averageFps:0.00}";
                _averageFps = 0f;
            }
        }
    }
}
