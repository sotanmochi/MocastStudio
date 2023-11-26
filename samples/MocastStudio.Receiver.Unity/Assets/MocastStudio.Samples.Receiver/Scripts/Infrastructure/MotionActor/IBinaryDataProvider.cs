using System.Threading;
using System.Threading.Tasks;

namespace MocastStudio.Samples.Receiver.Infrastructure.MotionActor
{
    public interface IBinaryDataProvider
    {
        Task<byte[]> LoadAsync(string path, CancellationToken cancellationToken = default);
    }
}
