using System.Threading;
using System.Threading.Tasks;

namespace MocapSignalTransmission.Infrastructure.MotionActor
{
    public interface ICharacterAvatarResourceProvider
    {
        Task<ICharacterAvatarResource> LoadAsync(string resourcePath, CancellationToken cancellationToken = default);
    }
}
