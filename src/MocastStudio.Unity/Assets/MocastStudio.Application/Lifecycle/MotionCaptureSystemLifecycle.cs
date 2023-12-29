using MocapSignalTransmission.MotionActor;
using MocapSignalTransmission.MotionDataSource;
using MocapSignalTransmission.Transmitter;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.Infrastructure.MotionActor;
using MocapSignalTransmission.Infrastructure.MotionDataSource;
using MocapSignalTransmission.Infrastructure.Transmitter;
using SignalStreaming.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class MotionCaptureSystemLifecycle : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(MotionCaptureSystemLifecycle)}] Configure");

            builder.Register<SignalStreamingClientFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterEntryPoint<MotionCaptureSystemEntryPoint>(Lifetime.Singleton);

            builder.Register<MotionActorService>(Lifetime.Singleton);
            builder.Register<MotionDataSourceService>(Lifetime.Singleton);
            builder.Register<TransmitterService>(Lifetime.Singleton);

            builder.Register<MotionActorServiceContext>(Lifetime.Singleton);
            builder.Register<MotionDataSourceServiceContext>(Lifetime.Singleton);
            builder.Register<TransmitterServiceContext>(Lifetime.Singleton);

            builder.Register<TransmitterFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MotionActorFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<VrmAvatarResourceProvider>(Lifetime.Singleton).AsImplementedInterfaces()
                .WithParameter<RenderPipelineType>(RenderPipelineType.UniversalRenderPipeline)
                .WithParameter<IBinaryDataProvider>(new LocalFileBinaryDataProvider());

            builder.Register<MotionDataSourceManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MocastStudioDataSourceManager>(Lifetime.Singleton).AsSelf();
            builder.Register<VMCProtocolDataSourceManager>(Lifetime.Singleton).AsSelf();
#if MOCOPI_RECEIVER_PLUGIN
            builder.Register<MocopiDataSourceManager>(Lifetime.Singleton).AsSelf();
#endif
#if MOTION_BUILDER_RECEIVER_PLUGIN
            builder.Register<MotionBuilderDataSourceManager>(Lifetime.Singleton).AsSelf();
#endif
        }
    }
}
