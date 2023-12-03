using MocastStudio.Universal.UIView.MotionActor;
using MocastStudio.Universal.UIView.MotionDataSource;
using MocastStudio.Universal.UIView.MotionSourceMapping;
using MocastStudio.Universal.UIView.Transmitter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Universal.Lifecycle
{
    public sealed class MotionCaptureUIViewLifecycle : LifetimeScope
    {
        [Header("UIView Components")]
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

            builder.RegisterComponent(_motionActorListView);
            builder.RegisterComponent(_motionActorLoaderView);
            builder.RegisterComponent(_motionDataSourceListView);
            builder.RegisterComponent(_motionDataSourceLoaderView);
            builder.RegisterComponent(_motionSourceMappingListView);
            builder.RegisterComponent(_transmitterListView);
            builder.RegisterComponent(_transmitterLoaderView);

            builder.RegisterEntryPoint<MotionActorPresenter>();
            // builder.RegisterEntryPoint<MotionDataSourcePresenter>();
            // builder.RegisterEntryPoint<MotionSourceMappingPresenter>();
            // builder.RegisterEntryPoint<TransmitterPresenter>();
        }
    }
}
