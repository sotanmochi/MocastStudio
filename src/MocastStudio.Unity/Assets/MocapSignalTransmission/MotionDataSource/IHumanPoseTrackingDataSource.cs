using HumanPose = UnityEngine.HumanPose;

namespace MocapSignalTransmission.MotionDataSource
{
    public interface IHumanPoseTrackingDataSource : IMotionDataSource
    {
        bool TryPeek(ref HumanPose humanPoseFrame);
    }
}
