using MocapSignalTransmission.MotionActor;
using MocapSignalTransmission.MotionDataSource;
using MocapSignalTransmission.Transmitter;
using MocapSignalTransmission.Infrastructure.BinaryDataProvider;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.Infrastructure.MotionActor;
using MocapSignalTransmission.Infrastructure.MotionDataSource;
using MocapSignalTransmission.Infrastructure.Transmitter;
// using SignalStreaming.Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class MotionCaptureSystemLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(MotionCaptureSystemLifetimeScope)}] Configure");

            // builder.Register<SignalStreamingClientFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterEntryPoint<MotionCaptureSystemLifecycle>(Lifetime.Singleton);

            builder.Register<MotionActorService>(Lifetime.Singleton);
            builder.Register<MotionDataSourceService>(Lifetime.Singleton);
            builder.Register<TransmitterService>(Lifetime.Singleton);

            builder.Register<MotionActorServiceContext>(Lifetime.Singleton);
            builder.Register<MotionDataSourceServiceContext>(Lifetime.Singleton);
            builder.Register<TransmitterServiceContext>(Lifetime.Singleton);

            builder.Register<TransmitterFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MotionActorFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<CompositeBinaryDataProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LocalFileBinaryDataProvider>(Lifetime.Singleton);
            builder.Register<StreamingAssetBinaryDataProvider>(Lifetime.Singleton);

            builder.Register<VrmAvatarResourceProvider>(Lifetime.Singleton).AsImplementedInterfaces()
                .WithParameter<RenderPipelineType>(RenderPipelineType.UniversalRenderPipeline);

            builder.Register<MotionDataSourceManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<VMCProtocolDataSourceManager>(Lifetime.Singleton).AsSelf();
#if MOCOPI_RECEIVER_PLUGIN
            builder.Register<MocopiDataSourceManager>(Lifetime.Singleton).AsSelf();
#endif
#if MOTION_BUILDER_RECEIVER_PLUGIN
            builder.Register<MotionBuilderDataSourceManager>(Lifetime.Singleton).AsSelf();
#endif
            // builder.Register<MocastStudioDataSourceManager>(Lifetime.Singleton).AsSelf()
            //     .WithParameter<int>((int)SignalTransportType.ENet);
        }
    }
}
