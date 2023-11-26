using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace MocastStudio.Samples.Receiver.Infrastructure.MotionActor
{
    public sealed class LocalFileBinaryDataProvider : IBinaryDataProvider
    {
        public async Task<byte[]> LoadAsync(string path, CancellationToken cancellationToken = default)
        {
            byte[] data;

            // NOTE: Enable to load streaming asset files on Android.
            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                var request = UnityWebRequest.Get(path);
                await request.SendWebRequest();
                data = request.downloadHandler.data;
            }
            else
            {
                data = await File.ReadAllBytesAsync(path, cancellationToken);
            }

            return data;
        }
    }
}
