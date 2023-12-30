using System;
using HumanPose = UnityEngine.HumanPose;
using MocapSignalTransmission.Transmitter;

namespace MocapSignalTransmission.Infrastructure.Transmitter.Serialization
{
    public sealed class HumanPoseOscSerializer : ISerializer
    {
        //
        // Data Structure:
        //  - int   x  1: ActorId
        //  - float x  3: HumanPose.bodyPosition
        //  - float x  4: HumanPose.bodyRotation
        //  - float x 95: HumanPose.muscles
        //
        // References:
        //  - https://gist.github.com/neon-izm/0637dac7a29682de916cecc0e8b037b0
        // 
        private static readonly string ValueTypes = ",iffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

        private readonly OscPacketEncoder _encoder = new();

        public string MessageAddress { get; set; }

        public HumanPoseOscSerializer()
        {
            MessageAddress = "/HumanPose";
        }

        public ArraySegment<byte> Serialize<T>(int actorId, T value)
        {
            if (typeof(T) != typeof(HumanPose))
            {
                throw new ArgumentException($"Invalid value type: {typeof(T)}");
            }

            var humanPose = (HumanPose)(object)value;

            _encoder.Clear();

            _encoder.Append(MessageAddress);
            _encoder.Append(ValueTypes);

            _encoder.Append(actorId);

            _encoder.Append(humanPose.bodyPosition.x);
            _encoder.Append(humanPose.bodyPosition.y);
            _encoder.Append(humanPose.bodyPosition.z);

            _encoder.Append(humanPose.bodyRotation.x);
            _encoder.Append(humanPose.bodyRotation.y);
            _encoder.Append(humanPose.bodyRotation.z);
            _encoder.Append(humanPose.bodyRotation.w);

            foreach (var muscle in humanPose.muscles)
            {
                _encoder.Append(muscle);
            }

            return new ArraySegment<byte>(_encoder.Buffer, 0, _encoder.Length);
        }
    }
}
