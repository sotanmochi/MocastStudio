using System.Threading;
using System.Threading.Tasks;

namespace MocapSignalTransmission.BinaryDataProvider
{
    public interface IBinaryDataProvider
    {
        Task<byte[]> LoadAsync<T>(T request, CancellationToken cancellationToken = default) where T : IBinaryDataLoadingRequest;
    }
}
