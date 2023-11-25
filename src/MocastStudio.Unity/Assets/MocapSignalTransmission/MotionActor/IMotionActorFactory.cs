using System.Threading;
using System.Threading.Tasks;

namespace MocapSignalTransmission.MotionActor
{
    public interface IMotionActorFactory
    {
        Task<HumanoidMotionActor> CreateAsync(int actorId, string resourcePath, CancellationToken cancellationToken = default);
    }
}
