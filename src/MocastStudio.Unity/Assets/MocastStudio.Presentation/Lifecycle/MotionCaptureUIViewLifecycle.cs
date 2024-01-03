using MocastStudio.Presentation.UIView.MotionActor;
using MocastStudio.Presentation.UIView.MotionCapture;
using MocastStudio.Presentation.UIView.MotionDataSource;
using MocastStudio.Presentation.UIView.MotionSourceMapping;
using MocastStudio.Presentation.UIView.Transmitter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Presentation.Lifecycle
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

            builder.RegisterComponent(_motionCaptureSystemView);
            builder.RegisterComponent(_motionActorListView);
            builder.RegisterComponent(_motionActorLoaderView);
            builder.RegisterComponent(_motionDataSourceListView);
            builder.RegisterComponent(_motionDataSourceLoaderView);
            builder.RegisterComponent(_motionSourceMappingListView);
            builder.RegisterComponent(_transmitterListView);
            builder.RegisterComponent(_transmitterLoaderView);

            builder.RegisterEntryPoint<MotionCaptureSystemPresenter>();
            builder.RegisterEntryPoint<MotionActorPresenter>();
            builder.RegisterEntryPoint<MotionDataSourcePresenter>();
            builder.RegisterEntryPoint<MotionSourceMappingPresenter>();
            builder.RegisterEntryPoint<TransmitterPresenter>();
        }
    }
}
