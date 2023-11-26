using System.Threading;
using System.Threading.Tasks;

namespace MocastStudio.Samples.Receiver.Domain.MotionActor
{
    public interface IMotionActorFactory
    {
        Task<HumanoidMotionActor> CreateAsync(string resourcePath, CancellationToken cancellationToken = default);
    }
}
