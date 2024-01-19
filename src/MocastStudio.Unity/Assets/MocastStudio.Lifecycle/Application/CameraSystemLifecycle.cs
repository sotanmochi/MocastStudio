using MocastStudio.Infrastructure.VideoFrameStreaming;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class CameraSystemLifecycle : LifetimeScope
    {
        [Header("Components")]
        [SerializeField] CameraSystemEntryPoint _cameraSystemEntryPoint;
        [SerializeField] SpoutVideoFrameStreamer _spoutVideoFrameStreamer;
        [SerializeField] SyphonVideoFrameStreamer _syphonVideoFrameStreamer;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(CameraSystemLifecycle)}] Configure");

            builder.RegisterComponent(_cameraSystemEntryPoint);

#if UNITY_STANDALONE_WIN
            builder.RegisterComponent(_spoutVideoFrameStreamer).AsImplementedInterfaces();
#elif UNITY_STANDALONE_OSX
            builder.RegisterComponent(_syphonVideoFrameStreamer).AsImplementedInterfaces();
#endif
        }
    }
}
