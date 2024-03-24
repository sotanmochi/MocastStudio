using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.BinaryDataProvider;

namespace MocapSignalTransmission.Infrastructure.MotionActor
{
    public interface ICharacterAvatarResourceProvider
    {
        Task<ICharacterAvatarResource> LoadAsync<T>(T request, CancellationToken cancellationToken = default) where T : IBinaryDataLoadingRequest;
    }
}
