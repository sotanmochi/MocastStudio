using System;
using MessagePack;
using SignalStreaming.Infrastructure.ENet;
using UnityEngine;

namespace SignalStreaming.Samples.ENetSample
{
    public class SampleServer : MonoBehaviour
    {
        [SerializeField] ushort _port = 3333;
        [SerializeField] string _connectionKey = "SignalStreaming";

        ISignalStreamingHub _streamingHub;
        ISignalTransportHub _transportHub;

        void Awake()
        {
            _transportHub = new ENetTransportHub(_port, useAnotherThread: true, targetFrameRate: 60, isBackground: true);
            _streamingHub = new SignalStreamingHub(_transportHub);

            _streamingHub.ConnectionRequestValidationFunc += IsValidConnectionRequest;
            _streamingHub.OnClientConnected += OnConnected;
            _streamingHub.OnClientDisconnected += OnDisconnected;
            _streamingHub.OnDataReceived += OnDataReceived;
        }

        void Start()
        {
            _streamingHub.Initialize();
        }

        void OnDestroy()
        {
            _streamingHub.ConnectionRequestValidationFunc -= IsValidConnectionRequest;
            _streamingHub.OnClientConnected -= OnConnected;
            _streamingHub.OnClientDisconnected -= OnDisconnected;
            _streamingHub.OnDataReceived -= OnDataReceived;

            _streamingHub.Dispose();
            _transportHub.Dispose();
        }

        RequestApprovalResult IsValidConnectionRequest(byte[] connectionRequestData)
        {
            if (connectionRequestData == null)
            {
                return new RequestApprovalResult(false, "Connection request is rejected. Invalid connection request data.");
            }

            var connectionKey = System.Text.Encoding.UTF8.GetString(connectionRequestData);

            var approved = (connectionKey == _connectionKey);
            var message = approved
                ? "Connection request is approved."
                : "Connection request is rejected. Invalid connection request data.";

            return new RequestApprovalResult(approved, message);
        }

        void OnConnected(uint clientId)
        {
            Debug.Log($"[{nameof(SampleServer)}] Connected - Client[{clientId}]");
        }

        void OnDisconnected(uint clientId)
        {
            Debug.Log($"[{nameof(SampleServer)}] Disconnected - Client[{clientId}]");
        }

        void OnDataReceived(int messageId, uint senderClientId, long originTimestamp, SendOptions sendOptions, ReadOnlyMemory<byte> payload)
        {
            Debug.Log($"[{nameof(SampleServer)}] Data received from Client[{senderClientId}]. " +
                $"Message ID: {messageId}, Payload.Length: {payload.Length}");

            if (messageId == 0)
            {
                var message = MessagePackSerializer.Deserialize<string>(payload);
                Debug.Log($"<color=lime>[{nameof(SampleServer)}] Received message: {message}</color>");
            }

            if (sendOptions.StreamingType == StreamingType.All)
            {
                _streamingHub.Broadcast(messageId, senderClientId, originTimestamp, payload, sendOptions.Reliable);
            }
        }
    }
}
