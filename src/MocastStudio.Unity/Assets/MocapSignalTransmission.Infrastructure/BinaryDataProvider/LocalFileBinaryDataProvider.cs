#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.BinaryDataProvider;

#if UNITY_ENGINE
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
#endif

namespace MocapSignalTransmission.Infrastructure.BinaryDataProvider
{
    public sealed class LocalFileBinaryDataProvider : IBinaryDataProvider
    {
        public async Task<byte[]> LoadAsync<T>(T request, CancellationToken cancellationToken = default) where T : IBinaryDataLoadingRequest
        {
            if (request is not LocalFileLoadingRequest loadingRequest)
            {
                return null;
            }

            var path = Path.Combine(loadingRequest.DirectoryPath, loadingRequest.Filename);
#if UNITY_ENGINE
            // NOTE: Enable to load streaming asset files on Android.
            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                var webRequest = UnityWebRequest.Get(path);
                await webRequest.SendWebRequest();
                return webRequest.downloadHandler.data;
            }
            else
#endif
            {
                return await File.ReadAllBytesAsync(path, cancellationToken);
            }
        }
    }
}
