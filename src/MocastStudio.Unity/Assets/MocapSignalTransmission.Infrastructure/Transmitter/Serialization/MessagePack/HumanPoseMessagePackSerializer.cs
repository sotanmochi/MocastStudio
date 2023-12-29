using System;
using MessagePack;
using MocapSignalTransmission.Transmitter;
using HumanPose = UnityEngine.HumanPose;

namespace MocapSignalTransmission.Infrastructure.Transmitter.Serialization
{
    public sealed class HumanPoseMessagePackSerializer : ISerializer
    {
        public ArraySegment<byte> Serialize<T>(int actorId, T value)
        {
            if (typeof(T) != typeof(HumanPose))
            {
                throw new ArgumentException($"Invalid value type: {typeof(T)}");
            }
            return new ArraySegment<byte>(MessagePackSerializer.Serialize(value));
        }
    }
}
