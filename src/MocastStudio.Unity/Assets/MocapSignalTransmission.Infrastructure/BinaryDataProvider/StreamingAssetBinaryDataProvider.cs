#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

#if UNITY_ENGINE
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MocapSignalTransmission.BinaryDataProvider;
using UnityEngine.Networking;

namespace MocapSignalTransmission.Infrastructure.BinaryDataProvider
{
    public class StreamingAssetBinaryDataProvider : IBinaryDataProvider
    {
        public async Task<byte[]> LoadAsync<T>(T request, CancellationToken cancellationToken = default) where T : IBinaryDataLoadingRequest
        {
            if (request is not StreamingAssetLoadingRequest loadingRequest)
            {
                return null;
            }

            var path = Path.Combine(UnityEngine.Application.streamingAssetsPath, loadingRequest.FolderName, loadingRequest.Filename);

            // NOTE: Enable to load streaming asset files on Android.
            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                var webRequest = UnityWebRequest.Get(path);
                await webRequest.SendWebRequest();
                return webRequest.downloadHandler.data;
            }
            else
            {
                return await File.ReadAllBytesAsync(path, cancellationToken);
            }
       }
    }
}
#endif