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
    public sealed class HumanPoseFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.HumanPose>
    {

        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::UnityEngine.HumanPose value, global::MessagePack.MessagePackSerializerOptions options)
        {
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector3>(formatterResolver).Serialize(ref writer, value.bodyPosition, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Quaternion>(formatterResolver).Serialize(ref writer, value.bodyRotation, options);
            global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<float[]>(formatterResolver).Serialize(ref writer, value.muscles, options);
        }

        public global::UnityEngine.HumanPose Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new global::System.InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var ____result = new global::UnityEngine.HumanPose();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        ____result.bodyPosition = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Vector3>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 1:
                        ____result.bodyRotation = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<global::UnityEngine.Quaternion>(formatterResolver).Deserialize(ref reader, options);
                        break;
                    case 2:
                        ____result.muscles = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<float[]>(formatterResolver).Deserialize(ref reader, options);
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
