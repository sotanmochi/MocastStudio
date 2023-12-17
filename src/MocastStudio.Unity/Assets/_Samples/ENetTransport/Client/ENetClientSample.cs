using System;
using UnityEngine;
using MocapSignalTransmission.Infrastructure.Transmitter.Transport;

namespace MocastStudio.Samples
{
    public sealed class ENetClientSample : MonoBehaviour
    {
        [SerializeField] bool _useAnotherThread;

        ENetTransport _client;
        float _previousTime;

        async void Start()
        {
            _client = new ENetTransport("localhost", 3333, reliable: true, useAnotherThread: _useAnotherThread, targetFrameRate: 60, isBackground: true);

            _client.OnConnected += () =>
            {
                Debug.Log($"[{nameof(ENetClientSample)}] Connected. @Thread: {System.Environment.CurrentManagedThreadId}");
            };
            _client.OnDisconnected += () =>
            {
                Debug.Log($"[{nameof(ENetClientSample)}] Disconnected. @Thread: {System.Environment.CurrentManagedThreadId}");
            };
            _client.OnDataReceived += data =>
            {
                var message = System.Text.Encoding.UTF8.GetString(data.Array, data.Offset, data.Count);
                Debug.Log($"[{nameof(ENetClientSample)}] Received message: {message} @Thread: {System.Environment.CurrentManagedThreadId}");
                Debug.Log($"[{nameof(ENetClientSample)}] Received data length: {data.Count}");
            };

            Debug.Log($"<color=yellow>[{nameof(ENetClientSample)}] Connecting...</color>");

            await _client.ConnectAsync();

            Debug.Log($"<color=lime>[{nameof(ENetClientSample)}] Connected.</color>");
            Debug.Log($"<color=lime>[{nameof(ENetClientSample)}] End of start</color>");
        }

        void Update()
        {
            if (!_useAnotherThread)
            {
                _client.PollEvent();
            }

            if (Time.realtimeSinceStartup - _previousTime > 1)
            {
                _previousTime = Time.realtimeSinceStartup;
                _client.Send(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes("Hello World!")));
                Debug.Log($"[{nameof(ENetClientSample)}] Send message. @Thread: {System.Environment.CurrentManagedThreadId}");
            }
        }

        void OnDestroy()
        {
            _client?.Dispose();
        }
    }
}
