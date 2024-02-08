using MocastStudio.Presentation.UIView.MotionActor;
using MocastStudio.Presentation.UIView.MotionCapture;
using MocastStudio.Presentation.UIView.MotionDataSource;
using MocastStudio.Presentation.UIView.MotionSourceMapping;
using MocastStudio.Presentation.UIView.Transmitter;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Lifecycle.Presentation
{
    public sealed class MotionCaptureUIViewLifecycle : IInitializable
    {
        readonly MotionCaptureSystemPresenter _motionCaptureSystemPresenter;
        readonly MotionActorPresenter _motionActorPresenter;
        readonly MotionDataSourcePresenter _motionDataSourcePresenter;
        readonly MotionSourceMappingPresenter _motionSourceMappingPresenter;
        readonly TransmitterPresenter _transmitterPresenter;

        public MotionCaptureUIViewLifecycle(
            MotionCaptureSystemPresenter motionCaptureSystemPresenter,
            MotionActorPresenter motionActorPresenter,
            MotionDataSourcePresenter motionDataSourcePresenter,
            MotionSourceMappingPresenter motionSourceMappingPresenter,
            TransmitterPresenter transmitterPresenter)
        {
            _motionCaptureSystemPresenter = motionCaptureSystemPresenter;
            _motionActorPresenter = motionActorPresenter;
            _motionDataSourcePresenter = motionDataSourcePresenter;
            _motionSourceMappingPresenter = motionSourceMappingPresenter;
            _transmitterPresenter = transmitterPresenter;
        }

        void IInitializable.Initialize()
        {
            _motionCaptureSystemPresenter.Initialize();
            _motionActorPresenter.Initialize();
            _motionDataSourcePresenter.Initialize();
            _motionSourceMappingPresenter.Initialize();
            _transmitterPresenter.Initialize();
        }
    }
}
