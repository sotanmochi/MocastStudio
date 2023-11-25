using MocapSignalTransmission.MotionData;

namespace MocapSignalTransmission.MotionDataSource
{
    public interface IBodyTrackingDataSource : IMotionDataSource
    {
        bool TryPeek(out BodyTrackingFrame data);
    }
}
