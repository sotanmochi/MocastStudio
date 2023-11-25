namespace MocapSignalTransmission.Transmitter
{
    public sealed class TransmitterSettings
    {
        public int TransmitterId { get; internal set; }
        public int SerializerType { get; }
        public int TransportType { get; }
        public string ServerAddress { get; }
        public int Port { get; }

        public TransmitterSettings(int serializerType, int transportType, string serverAddress, int port)
        {
            TransmitterId = -1;
            SerializerType = serializerType;
            TransportType = transportType;
            ServerAddress = serverAddress;
            Port = port;
        }
    }
}
