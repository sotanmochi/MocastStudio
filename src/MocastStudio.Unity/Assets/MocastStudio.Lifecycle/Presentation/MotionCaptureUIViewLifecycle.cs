using MocastStudio.Presentation.UIView.MotionActor;
using MocastStudio.Presentation.UIView.MotionCapture;
using MocastStudio.Presentation.UIView.MotionDataSource;
using MocastStudio.Presentation.UIView.MotionSourceMapping;
using MocastStudio.Presentation.UIView.Transmitter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Lifecycle.Presentation
{
    public sealed class MotionCaptureUIViewLifecycle : LifetimeScope
    {
        [Header("UIView Components")]
        [SerializeField] MotionCaptureSystemView _motionCaptureSystemView;
        [SerializeField] MotionActorListView _motionActorListView;
        [SerializeField] MotionActorLoaderView _motionActorLoaderView;
        [SerializeField] MotionDataSourceListView _motionDataSourceListView;
        [SerializeField] MotionDataSourceLoaderView _motionDataSourceLoaderView;
        [SerializeField] MotionSourceMappingListView _motionSourceMappingListView;
        [SerializeField] TransmitterListView  _transmitterListView;
        [SerializeField] TransmitterLoaderView  _transmitterLoaderView;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(MotionCaptureUIViewLifecycle)}] Configure");

            builder.RegisterEntryPoint<MotionCaptureUIViewEntryPoint>();

            builder.RegisterComponent(_motionCaptureSystemView);
            builder.RegisterComponent(_motionActorListView);
            builder.RegisterComponent(_motionActorLoaderView);
            builder.RegisterComponent(_motionDataSourceListView);
            builder.RegisterComponent(_motionDataSourceLoaderView);
            builder.RegisterComponent(_motionSourceMappingListView);
            builder.RegisterComponent(_transmitterListView);
            builder.RegisterComponent(_transmitterLoaderView);

            builder.Register<MotionCaptureSystemPresenter>(Lifetime.Singleton);
            builder.Register<MotionActorPresenter>(Lifetime.Singleton);
            builder.Register<MotionDataSourcePresenter>(Lifetime.Singleton);
            builder.Register<MotionSourceMappingPresenter>(Lifetime.Singleton);
            builder.Register<TransmitterPresenter>(Lifetime.Singleton);
        }
    }
}
