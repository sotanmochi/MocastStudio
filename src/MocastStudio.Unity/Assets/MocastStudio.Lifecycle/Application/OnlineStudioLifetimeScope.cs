using MessagePack;
using MocastStudio.Application.OnlineStudio;
using MocastStudio.Infrastructure.OnlineStudio;
using SignalStreaming;
using SignalStreaming.Serialization;
using SignalStreaming.Infrastructure.LiteNetLib;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Lifecycle
{
    public sealed class OnlineStudioLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(OnlineStudioLifetimeScope)}] Configure");

            builder.RegisterEntryPoint<OnlineStudioLifecycle>(Lifetime.Singleton);

            builder.Register<ConnectionServiceContext>(Lifetime.Singleton);
            builder.Register<ConnectionService>(Lifetime.Singleton);

            builder.Register<OnlineStudioEndpointProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SignalStreamingClient>(Lifetime.Singleton).AsImplementedInterfaces(); 

            builder.Register<SignalSerializer>(Lifetime.Singleton).AsImplementedInterfaces()
                .WithParameter(MessagePackSerializer.DefaultOptions);

            var transportThreadTargetFrameRate = 120;
            builder.Register<LiteNetLibTransport>(Lifetime.Singleton).AsImplementedInterfaces()
                .WithParameter(transportThreadTargetFrameRate);
        }
    }
}