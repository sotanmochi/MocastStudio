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
            if (typeof(T) == typeof(LocalFileLoadingRequest))
            {
                var loadingRequest = request as LocalFileLoadingRequest;
                if (loadingRequest == null) return null;
                return await _localFileBinaryDataProvider.LoadAsync(loadingRequest, cancellationToken);
            }
            else if (typeof(T) == typeof(StreamingAssetLoadingRequest))
            {
                var loadingRequest = request as StreamingAssetLoadingRequest;
                if (loadingRequest == null) return null;
                return await _streamingAssetBinaryDataProvider.LoadAsync(loadingRequest, cancellationToken);
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
