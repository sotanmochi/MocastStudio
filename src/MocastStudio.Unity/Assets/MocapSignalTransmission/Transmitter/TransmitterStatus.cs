namespace MocapSignalTransmission.Transmitter
{
    public readonly struct TransmitterStatus
    {
        public readonly int TransmitterId;
        public readonly TransmitterStatusType StatusType;

        public TransmitterStatus(int dataSourceId, TransmitterStatusType statusType)
        {
            TransmitterId = dataSourceId;
            StatusType = statusType;
        }
    }

    public enum TransmitterStatusType
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
    }
}
