using MocastStudio.Infrastructure.VideoFrameStreaming;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class VideoFrameStreamingLifetimeScope : LifetimeScope
    {
        [Header("Components")]
        [SerializeField] SpoutVideoFrameStreamer _spoutVideoFrameStreamer;
        [SerializeField] SyphonVideoFrameStreamer _syphonVideoFrameStreamer;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(VideoFrameStreamingLifetimeScope)}] Configure");

#if UNITY_STANDALONE_WIN
            builder.RegisterComponent(_spoutVideoFrameStreamer).AsImplementedInterfaces();
#elif UNITY_STANDALONE_OSX
            builder.RegisterComponent(_syphonVideoFrameStreamer).AsImplementedInterfaces();
#endif
        }
    }
}
