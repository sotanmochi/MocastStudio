namespace MocapSignalTransmission.Transmitter
{
    public readonly struct TransmitterStatus
    {
        public readonly int TransmitterId;
        public readonly int StatusType;

        public TransmitterStatus(int dataSourceId, TransmitterStatusType statusType)
        {
            TransmitterId = dataSourceId;
            StatusType = (int)statusType;
        }
    }

    public enum TransmitterStatusType
    {
        Disconnected = -1,
        Connecting = 0,
        Connected = 1,
    }
}
