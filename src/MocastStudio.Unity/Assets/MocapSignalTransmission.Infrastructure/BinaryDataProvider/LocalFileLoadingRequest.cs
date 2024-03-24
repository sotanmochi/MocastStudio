using MocapSignalTransmission.BinaryDataProvider;

namespace MocapSignalTransmission.Infrastructure.BinaryDataProvider
{
    public sealed class LocalFileLoadingRequest : IBinaryDataLoadingRequest
    {
        public string DirectoryPath { get; set; }
        public string Filename { get; set; }
    }
}
