using System.Threading;
using System.Threading.Tasks;
using MocastStudio.Samples.Receiver.Domain.MotionActor;

namespace MocastStudio.Samples.Receiver.Infrastructure.MotionActor
{
    public interface ICharacterAvatarResourceProvider
    {
        Task<ICharacterAvatarResource> LoadAsync(string resourcePath, CancellationToken cancellationToken = default);
    }
}
