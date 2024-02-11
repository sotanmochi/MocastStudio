using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using SignalStreaming;
using SignalStreaming.Infrastructure.ENet;

namespace SignalStreamingSamples.ConsoleAppClient
{
    public class SampleClient
    {
        string _serverAddress = "localhost";
        ushort _port = 3333;
        string _connectionKey = "SignalStreaming";

        ISignalTransport _transport;
        ISignalStreamingClient _streamingClient;

        public SampleClient(string serverAddress, ushort port, string connectionKey)
        {
            _serverAddress = serverAddress;
            _port = port;
            _connectionKey = connectionKey;

            _transport = new ENetTransport(useAnotherThread: true, targetFrameRate: 60, isBackground: true);
            _transport.OnConnected += () => Console.WriteLine($"[{nameof(ConsoleAppClient)}] TransportConnected");
            _transport.OnDisconnected += () => Console.WriteLine($"[{nameof(ConsoleAppClient)}] TransportDisconnected");
            _transport.OnDataReceived += (payload) => Console.WriteLine($"[{nameof(ConsoleAppClient)}] TransportDataReceived - Payload.Length: {payload.Count}");

            _streamingClient = new SignalStreamingClient(_transport);
            _streamingClient.OnConnected += OnConnected;
            _streamingClient.OnDisconnected += OnDisconnected;
            _streamingClient.OnDataReceived += OnDataReceived;
        }

        public async void StartAsync()
        {
            var connectParameters = new ENetConnectParameters()
            {
                ConnectionRequestData = System.Text.Encoding.UTF8.GetBytes(_connectionKey),
                ServerAddress = _serverAddress,
                ServerPort = _port
            };

            Log($"[{nameof(ConsoleAppClient)}] StreamingClient is connecting... @Thread: {Thread.CurrentThread.ManagedThreadId}");

            var connected = await _streamingClient.ConnectAsync(connectParameters);

            Log($"[{nameof(ConsoleAppClient)}] StreamingClient.IsConnected: {connected} @Thread: {Thread.CurrentThread.ManagedThreadId}");
        }

        void OnConnected(uint clientId)
        {
            Log($"[{nameof(ConsoleAppClient)}] Connected - ClientId: {clientId}");
        }

        void OnDisconnected(string reason)
        {
            Log($"[{nameof(ConsoleAppClient)}] Disconnected - Reason: {reason}");
        }

        void OnGroupJoined()
        {
            Log($"[{nameof(ConsoleAppClient)}] GroupJoined");
        }

        void OnGroupLeft()
        {
            Log($"[{nameof(ConsoleAppClient)}] GroupLeft");
        }

        void OnDataReceived(int messageId, uint senderClientId, long originTimestamp, long transmitTimestamp, ReadOnlyMemory<byte> payload)
        {
            var originDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(originTimestamp).ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            var transmitDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(transmitTimestamp).ToString("MM/dd/yyyy hh:mm:ss.fff tt");

            Log($"[{nameof(ConsoleAppClient)}] Received data sent from client[{senderClientId}]. " +
                $"Message ID: {messageId}, Payload.Length: {payload.Length}, " +
                $"OriginTimestamp: {originDateTimeOffset}, TransmitTimestamp: {transmitDateTimeOffset}");

            if (messageId == 0)
            {
                var text = MessagePackSerializer.Deserialize<string>(payload);
                Log($"<color=yello>[{nameof(ConsoleAppClient)}] Received message: {text}</color>");
            }
        }

        void Log(object message)
        {
            Console.WriteLine(message);
        }
    }
}
