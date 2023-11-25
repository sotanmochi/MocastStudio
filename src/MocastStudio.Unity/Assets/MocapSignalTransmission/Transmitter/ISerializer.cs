using System;

namespace MocapSignalTransmission.Transmitter
{
    public interface ISerializer
    {
        ArraySegment<byte> Serialize<T>(int actorId, T value);
    }
}
