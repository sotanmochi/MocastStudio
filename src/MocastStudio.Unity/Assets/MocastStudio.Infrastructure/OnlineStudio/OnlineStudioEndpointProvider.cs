using System.Threading;
using MocastStudio.Application.OnlineStudio;

namespace MocastStudio.Infrastructure.OnlineStudio
{
    public sealed class OnlineStudioEndpointProvider : IOnlineStudioEndpointProvider
    {
        public EndPoint FindBySignalStreamingGroupIdAsync(string signalStreamingGroupId, CancellationToken cancellationToken = default)
        {
            return new EndPoint(){ Address = "localhost", Port = 54970 };
        }
    }
}
