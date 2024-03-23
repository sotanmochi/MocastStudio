using SignalStreaming;
using R3;

namespace MocastStudio.Application.OnlineStudio
{
    public sealed class ConnectionServiceContext
    {
        internal uint streamingClientId;

        internal ReactiveProperty<bool> connected = new();
        internal ReactiveProperty<bool> joined = new();

        public ReadOnlyReactiveProperty<bool> Connected => connected;
        public ReadOnlyReactiveProperty<bool> Joined => joined;

        public string GroupId { get; internal set; }
    }
}