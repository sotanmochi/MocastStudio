using MocastStudio.Infrastructure.VideoFrameStreaming;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class CameraSystemLifetimeScope : LifetimeScope
    {
        [Header("Components")]
        [SerializeField] CameraSystemLifecycle _cameraSystemLifecycle;
        [SerializeField] SpoutVideoFrameStreamer _spoutVideoFrameStreamer;
        [SerializeField] SyphonVideoFrameStreamer _syphonVideoFrameStreamer;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(CameraSystemLifetimeScope)}] Configure");

            builder.RegisterComponent(_cameraSystemLifecycle);

#if UNITY_STANDALONE_WIN
            builder.RegisterComponent(_spoutVideoFrameStreamer).AsImplementedInterfaces();
#elif UNITY_STANDALONE_OSX
            builder.RegisterComponent(_syphonVideoFrameStreamer).AsImplementedInterfaces();
#endif
        }
    }
}
