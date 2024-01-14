using System;
using System.Buffers;
using MocapSignalTransmission.MotionActor;
using MocapSignalTransmission.Transmitter;

namespace MocapSignalTransmission.Infrastructure.Transmitter.Serialization
{
    //
    // This code is based on the VMC Protocol specification.
    // References:
    //  - https://protocol.vmc.info/marionette-spec
    //
    public sealed class HumanoidMotionActorVmcpSerializer : ISerializer
    {
        const string OscBundleIdentifier = "#bundle";

        const string OscAddressOfTimeData = "/VMC/Ext/T";
        const string OscAddressOfRootTransformData = "/VMC/Ext/Root/Pos";
        const string OscAddressOfBoneTransformData = "/VMC/Ext/Bone/Pos";

        const string ValueTypesOfTimeData = ",f";
        const string ValueTypesOfTransformData = ",sfffffff";

        const string RootBoneName = "Root";

        readonly OscPacketEncoder _encoder = new();
        readonly int _bufferSizeOfTimeMessage;
        readonly int _bufferSizeOfRootTransformMessage;
        readonly int _baseBufferSizeOfBoneTransformMessage;

        public HumanoidMotionActorVmcpSerializer()
        {
            _bufferSizeOfTimeMessage = GetRequiredStringBufferSize(OscAddressOfTimeData)
                                        + GetRequiredStringBufferSize(ValueTypesOfTimeData)
                                        + 4; // 32-bit float

            _bufferSizeOfRootTransformMessage = GetRequiredStringBufferSize(OscAddressOfRootTransformData)
                                                + GetRequiredStringBufferSize(ValueTypesOfTransformData)
                                                + GetRequiredStringBufferSize(RootBoneName)
                                                + 4 * 3  // 32-bit float x 3
                                                + 4 * 4; // 32-bit float x 4

            _baseBufferSizeOfBoneTransformMessage = GetRequiredStringBufferSize(OscAddressOfBoneTransformData)
                                                    + GetRequiredStringBufferSize(ValueTypesOfTransformData)
                                                    + 4 * 3  // 32-bit float x 3
                                                    + 4 * 4; // 32-bit float x 4
        }

        public ReadOnlySequence<byte> Serialize<T>(uint actorId, T value)
        {
            if (typeof(T) != typeof(HumanoidMotionActor))
            {
                throw new ArgumentException($"Invalid value type: {typeof(T)}");
            }

            var humanoidMotionActor = (HumanoidMotionActor)(object)value; // TODO: Avoid allocation

            _encoder.Clear();

            _encoder.Append(OscBundleIdentifier);

            // We don't use timestamp (64-bit unsigned integer)
            _encoder.Append(0);
            _encoder.Append(0);

            // Time
            _encoder.Append(_bufferSizeOfTimeMessage);
            _encoder.Append(OscAddressOfTimeData);
            _encoder.Append(ValueTypesOfTimeData);
            _encoder.Append(0f);

            // Root transform
            var rootPosition = humanoidMotionActor.ActorSpaceReferenceTransform.position;
            var rootRotation = humanoidMotionActor.ActorSpaceReferenceTransform.rotation;
            _encoder.Append(_bufferSizeOfRootTransformMessage);
            _encoder.Append(OscAddressOfRootTransformData);
            _encoder.Append(ValueTypesOfTransformData);
            _encoder.Append(RootBoneName);
            _encoder.Append(rootPosition.x);
            _encoder.Append(rootPosition.y);
            _encoder.Append(rootPosition.z);
            _encoder.Append(rootRotation.x);
            _encoder.Append(rootRotation.y);
            _encoder.Append(rootRotation.z);
            _encoder.Append(rootRotation.w);

            // Bone transforms
            for (var i = 0; i < humanoidMotionActor.BodyBones.Length; i++)
            {
                var boneName = humanoidMotionActor.BodyBones[i].Name;
                var localPosition = humanoidMotionActor.BodyBones[i].Transform.localPosition;
                var localRotation = humanoidMotionActor.BodyBones[i].Transform.localRotation;

                var sizeOfBoneName = GetRequiredStringBufferSize(boneName);

                _encoder.Append(_baseBufferSizeOfBoneTransformMessage + sizeOfBoneName);
                _encoder.Append(OscAddressOfBoneTransformData);
                _encoder.Append(ValueTypesOfTransformData);
                _encoder.Append(boneName);
                _encoder.Append(localPosition.x);
                _encoder.Append(localPosition.y);
                _encoder.Append(localPosition.z);
                _encoder.Append(localRotation.x);
                _encoder.Append(localRotation.y);
                _encoder.Append(localRotation.z);
                _encoder.Append(localRotation.w);
            }
            for (var i = 0; i < humanoidMotionActor.FingerBones.Length; i++)
            {
                var boneName = humanoidMotionActor.FingerBones[i].Name;
                var localPosition = humanoidMotionActor.FingerBones[i].Transform.localPosition;
                var localRotation = humanoidMotionActor.FingerBones[i].Transform.localRotation;

                var bufferSizeOfBoneName = GetRequiredStringBufferSize(boneName);

                _encoder.Append(_baseBufferSizeOfBoneTransformMessage + bufferSizeOfBoneName);
                _encoder.Append(OscAddressOfBoneTransformData);
                _encoder.Append(ValueTypesOfTransformData);
                _encoder.Append(boneName);
                _encoder.Append(localPosition.x);
                _encoder.Append(localPosition.y);
                _encoder.Append(localPosition.z);
                _encoder.Append(localRotation.x);
                _encoder.Append(localRotation.y);
                _encoder.Append(localRotation.z);
                _encoder.Append(localRotation.w);;
            }

            return new ReadOnlySequence<byte>(_encoder.Buffer, 0, _encoder.Length);
        }

        static int GetRequiredStringBufferSize(string data)
        {
            return OscDataTypes.Align4(data.Length + 1);
        }
    }
}
