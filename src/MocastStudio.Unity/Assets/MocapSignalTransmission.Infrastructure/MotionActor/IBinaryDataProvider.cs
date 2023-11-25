using System.Threading;
using System.Threading.Tasks;

namespace MocapSignalTransmission.Infrastructure.MotionActor
{
    public interface IBinaryDataProvider
    {
        Task<byte[]> LoadAsync(string path, CancellationToken cancellationToken = default);
    }
}
