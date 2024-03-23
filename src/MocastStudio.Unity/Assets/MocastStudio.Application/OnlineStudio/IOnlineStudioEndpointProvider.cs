using System.Threading;

namespace MocastStudio.Application.OnlineStudio
{
    public interface IOnlineStudioEndpointProvider
    {
        EndPoint FindBySignalStreamingGroupIdAsync(string signalStreamingGroupId, CancellationToken cancellationToken = default);
    }
}
