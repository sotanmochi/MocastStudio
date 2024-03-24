using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.BinaryDataProvider;

namespace MocapSignalTransmission.MotionActor
{
    public interface IMotionActorFactory
    {
        Task<HumanoidMotionActor> CreateAsync(int actorId, IBinaryDataLoadingRequest request, CancellationToken cancellationToken = default);
    }
}
