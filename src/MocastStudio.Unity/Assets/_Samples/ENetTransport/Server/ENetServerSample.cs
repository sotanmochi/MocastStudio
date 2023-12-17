using System.Collections;
using System.Collections.Generic;
using SignalTransport.ENet;
using UnityEngine;

namespace MocastStudio.Samples
{
    public sealed class ENetServerSample : MonoBehaviour
    {
        ENetServer _server;

        void Start()
        {
            _server = new ENetServer(port: 3333, targetFrameRate: 250, isBackground: true);
            _server.OnConnected += clientId => Debug.Log($"[{nameof(ENetServerSample)}] Client connected: {clientId}");
            _server.OnDisconnected += clientId => Debug.Log($"[{nameof(ENetServerSample)}] Client disconnected: {clientId}");
            _server.OnDataReceived += values =>
            {
                _server.Broadcast(values.Data, reliable: true);

                var clientId = values.ClientId;
                var data = values.Data;
                var message = System.Text.Encoding.UTF8.GetString(data.Array, data.Offset, data.Count);

                Debug.Log($"[{nameof(ENetServerSample)}] Received data from client: {clientId}");
                Debug.Log($"[{nameof(ENetServerSample)}] Received data length: {data.Count}");
                Debug.Log($"[{nameof(ENetServerSample)}] Received message: {message}");
            };
        }

        void OnDestroy()
        {
            Debug.Log($"[{nameof(ENetServerSample)}] Shutting down server...");
            _server?.Shutdown();
        }
    }
}
