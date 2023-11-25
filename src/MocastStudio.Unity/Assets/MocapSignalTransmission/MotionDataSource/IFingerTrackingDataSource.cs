using MocapSignalTransmission.MotionData;

namespace MocapSignalTransmission.MotionDataSource
{
    public interface IFingerTrackingDataSource : IMotionDataSource
    {
        bool TryPeek(out FingerTrackingFrame data);
    }
}
