#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.MocapSignalTransmission
{
    public sealed class ActorHumanPoseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::MocapSignalTransmission.MotionData.ActorHumanPose>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::MocapSignalTransmission.MotionData.ActorHumanPose value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(4);
            writer.Write(value.ActorId);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<UnityEngine.Vector3>(formatterResolver).Serialize(ref writer, value.BodyPosition, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<UnityEngine.Quaternion>(formatterResolver).Serialize(ref writer, value.BodyRotation, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<float[]>(formatterResolver).Serialize(ref writer, value.Muscles, options);
        }

        public global::MocapSignalTransmission.MotionData.ActorHumanPose Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::MocapSignalTransmission.MotionData.ActorHumanPose();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.ActorId = reader.ReadUInt32();
                        break;
                    case 1:
                        ____result.BodyPosition = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<UnityEngine.Vector3>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.BodyRotation = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<UnityEngine.Quaternion>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 3:
                        ____result.Muscles = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<float[]>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
