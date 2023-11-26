using MocastStudio.Samples.Receiver.Domain.MotionActor;
using MocastStudio.Samples.Receiver.Infrastructure.Constants;
using MocastStudio.Samples.Receiver.Infrastructure.MotionActor;
using MocastStudio.Samples.Receiver.UIView;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Samples.Receiver.Application
{
    public sealed class AppMainLifecycle : LifetimeScope
    {
        [SerializeField] private MotionActorLoaderUIView _motionActorLoaderUIView;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(AppMainLifecycle)}] Configure");

            builder.RegisterEntryPoint<AppMain>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MotionActorLoaderPresenter>(Lifetime.Singleton);

            builder.Register<MotionActorService>(Lifetime.Singleton);

            builder.Register<MotionActorFactory>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<VrmAvatarResourceProvider>(Lifetime.Singleton).AsImplementedInterfaces()
                    .WithParameter<RenderPipelineType>(RenderPipelineType.UniversalRenderPipeline)
                    .WithParameter<IBinaryDataProvider>(new LocalFileBinaryDataProvider());

            builder.RegisterComponent(_motionActorLoaderUIView);
        }
    }
}
