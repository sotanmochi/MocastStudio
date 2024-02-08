using MocapSignalTransmission.MotionActor;
using MocapSignalTransmission.MotionDataSource;
using MocapSignalTransmission.Transmitter;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class MotionCaptureSystemLifecycle : IPostLateTickable
    {
        private readonly MotionActorService _motionActorService;
        private readonly TransmitterService _transmitterService;
        private readonly MotionActorServiceContext _motionActorServiceContext;
        private readonly MotionDataSourceServiceContext _motionDataSourceServiceContext;

        public MotionCaptureSystemLifecycle(
            MotionActorService motionActorService,
            TransmitterService transmitterService,
            MotionActorServiceContext motionActorServiceContext,
            MotionDataSourceServiceContext motionDataSourceServiceContext)
        {
            _motionActorService = motionActorService;
            _transmitterService = transmitterService;
            _motionActorServiceContext = motionActorServiceContext;
            _motionDataSourceServiceContext = motionDataSourceServiceContext;
        }
 
        void IPostLateTickable.PostLateTick()
        {
            _motionActorService.UpdateMotionActorPose(
                _motionDataSourceServiceContext.BodyTrackingDataSources,
                _motionDataSourceServiceContext.FingerTrackingDataSources);

            _motionActorService.UpdateMotionActorPose(_motionDataSourceServiceContext.HumanPoseTrackingDataSources);

            _motionActorService.UpdateHumanPose();

            _transmitterService.SendMotionActorPose(_motionActorServiceContext.MotionActors);
        }
    }
}
