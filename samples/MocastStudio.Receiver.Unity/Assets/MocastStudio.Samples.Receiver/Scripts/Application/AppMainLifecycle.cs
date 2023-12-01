using MessagePipe;
using MocastStudio.Samples.Receiver.Domain.MotionActor;
using MocastStudio.Samples.Receiver.Infrastructure.Constants;
using MocastStudio.Samples.Receiver.Infrastructure.MotionActor;
using MocastStudio.Samples.Receiver.Infrastructure.StreamingReceiver;
using MocastStudio.Samples.Receiver.UIView;
using UnityEngine;
using uOSC;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Samples.Receiver.Application
{
    public sealed class AppMainLifecycle : LifetimeScope
    {
        [SerializeField] private MotionActorLoaderUIView _motionActorLoaderUIView;
        [SerializeField] private uOscServer _oscServer;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(AppMainLifecycle)}] Configure");

            ConfigureMessagePipe(builder);

            builder.RegisterEntryPoint<AppMain>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MotionActorLoaderPresenter>(Lifetime.Singleton);

            builder.Register<MotionActorService>(Lifetime.Singleton);

            builder.Register<MotionActorFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<VrmAvatarResourceProvider>(Lifetime.Singleton).AsImplementedInterfaces()
                    .WithParameter<RenderPipelineType>(RenderPipelineType.UniversalRenderPipeline)
                    .WithParameter<IBinaryDataProvider>(new LocalFileBinaryDataProvider());

            builder.Register<HumanPoseStreamingReceiver>(Lifetime.Singleton);

            builder.RegisterComponent(_oscServer);
            builder.RegisterComponent(_motionActorLoaderUIView);
        }

        private void ConfigureMessagePipe(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe(/* configure option */);

            // Setup GlobalMessagePipe to enable diagnostics window and global function
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            // Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.
            builder.RegisterMessageBroker<HumanPose>(options);
        }
    }
}
