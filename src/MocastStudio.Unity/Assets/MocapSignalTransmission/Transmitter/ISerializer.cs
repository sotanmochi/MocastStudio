using System;
using System.Buffers;

namespace MocapSignalTransmission.Transmitter
{
    public interface ISerializer
    {
        ReadOnlySequence<byte> Serialize<T>(int actorId, T value);
    }
}
