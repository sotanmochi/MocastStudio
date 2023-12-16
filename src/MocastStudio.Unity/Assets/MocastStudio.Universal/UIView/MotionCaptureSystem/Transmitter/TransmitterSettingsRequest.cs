using System.Collections.Generic;

namespace MocastStudio.Universal.UIView.Transmitter
{
    public sealed class TransmitterSettingsRequest
    {
        public TransmitterType TransmitterType { get; }
        public IReadOnlyList<int> ActorIds { get; }
        public string ServerAddress { get; }
        public int Port { get; }

        public TransmitterSettingsRequest(TransmitterType transmitterType,  IReadOnlyList<int> actorIds, string serverAddress, int port)
        {
            TransmitterType = transmitterType;
            ActorIds = actorIds;
            ServerAddress = serverAddress;
            Port = port;
        }
    }
}
