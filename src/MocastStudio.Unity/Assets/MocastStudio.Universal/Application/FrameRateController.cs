// References:
//  - https://blog.unity.com/ja/engine-platform/precise-framerates-in-unity
//  - https://blog.unity.com/engine-platform/precise-framerates-in-unity

using System.Collections;
using System.Threading;
using UnityEngine;

namespace MocastStudio.Universal.Application
{
    public sealed class FrameRateController : MonoBehaviour
    {
        private readonly WaitForEndOfFrame _waitForEndOfFrame = new();

        private float _currentFrameTime;
        private float _targetFrameRate = 60.0f;

        public float TargetFrameRate
        {
            get
            {
                return _targetFrameRate;
            }
            set
            {
                if (value > 0f && value < 9999f) _targetFrameRate = value;
            }
        }

        void Awake()
        {
            UnityEngine.QualitySettings.vSyncCount = 0;
            UnityEngine.Application.targetFrameRate = 9999;
            _currentFrameTime = Time.realtimeSinceStartup;
            StartCoroutine("WaitForNextFrame");
        }

        IEnumerator WaitForNextFrame()
        {
            while (true)
            {
                yield return _waitForEndOfFrame;
                _currentFrameTime += 1.0f / TargetFrameRate;
                var t = Time.realtimeSinceStartup;
                var sleepTime = _currentFrameTime - t - 0.01f;
                if (sleepTime > 0)
                    Thread.Sleep((int)(sleepTime * 1000));
                while (t < _currentFrameTime)
                    t = Time.realtimeSinceStartup;
            }
        }
    }
}
