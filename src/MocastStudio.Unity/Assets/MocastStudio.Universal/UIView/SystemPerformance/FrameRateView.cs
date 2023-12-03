using UnityEngine;
using Text = TMPro.TMP_Text;

namespace MocastStudio.Universal.UIView.SystemPerformance
{
    public sealed class FrameRateView : MonoBehaviour
    {
        [SerializeField] Text _fpsText;

        private float _previousFrameTime;

        void Update()
        {
            var currentFrameTime = Time.realtimeSinceStartup;
            var deltaTime = currentFrameTime - _previousFrameTime;
            _previousFrameTime = currentFrameTime;

            var fps = 1f / deltaTime;
            _fpsText.text = $"FPS: {fps:0.00}";
        }
    }
}
