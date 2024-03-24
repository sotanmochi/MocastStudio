using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.BinaryDataProvider;

namespace MocapSignalTransmission.Infrastructure.BinaryDataProvider
{
    public sealed class CompositeBinaryDataProvider : IBinaryDataProvider
    {
        readonly LocalFileBinaryDataProvider _localFileBinaryDataProvider;
        readonly StreamingAssetBinaryDataProvider _streamingAssetBinaryDataProvider;

        public CompositeBinaryDataProvider(
            LocalFileBinaryDataProvider localFileBinaryDataProvider,
            StreamingAssetBinaryDataProvider streamingAssetBinaryDataProvider)
        {
            _localFileBinaryDataProvider = localFileBinaryDataProvider;
            _streamingAssetBinaryDataProvider = streamingAssetBinaryDataProvider;
        }

        public async Task<byte[]> LoadAsync<T>(T request, CancellationToken cancellationToken = default) where T : IBinaryDataLoadingRequest
        {
            if (request is LocalFileLoadingRequest localFileLoadingRequest)
            {
                if (localFileLoadingRequest == null) return null;
                return await _localFileBinaryDataProvider.LoadAsync(localFileLoadingRequest, cancellationToken);
            }
            else if (request is StreamingAssetLoadingRequest streamingAssetLoadingRequest)
            {
                if (streamingAssetLoadingRequest == null) return null;
                return await _streamingAssetBinaryDataProvider.LoadAsync(streamingAssetLoadingRequest, cancellationToken);
            }
            else
            {
                Log($"Unsupported loading request type: {typeof(T)}");
                return null;
            }
        }

        void Log(object message)
        {
            UnityEngine.Debug.Log($"[{nameof(CompositeBinaryDataProvider)}] {message}");
        }
    }
}
