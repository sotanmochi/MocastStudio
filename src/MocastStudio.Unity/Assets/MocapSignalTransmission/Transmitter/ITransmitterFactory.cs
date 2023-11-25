namespace MocapSignalTransmission.Transmitter
{
    public interface ITransmitterFactory
    {
        ITransmitter Create(int transmitterId, TransmitterSettings settings);
    }
}
