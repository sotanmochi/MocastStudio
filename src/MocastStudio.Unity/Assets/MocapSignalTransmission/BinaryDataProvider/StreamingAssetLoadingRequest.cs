using MocapSignalTransmission.BinaryDataProvider;

namespace MocapSignalTransmission.BinaryDataProvider
{
    public sealed class StreamingAssetLoadingRequest : IBinaryDataLoadingRequest
    {
        public string Filename { get; set; }
    }
}
