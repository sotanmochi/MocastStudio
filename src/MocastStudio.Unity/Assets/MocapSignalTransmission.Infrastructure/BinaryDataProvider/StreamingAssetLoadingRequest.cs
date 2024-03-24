using MocapSignalTransmission.BinaryDataProvider;

namespace MocapSignalTransmission.Infrastructure.BinaryDataProvider
{
    public sealed class StreamingAssetLoadingRequest : IBinaryDataLoadingRequest
    {
        public string Filename { get; set; }
    }
}
