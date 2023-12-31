using System;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using SignalStreaming;
using SignalStreaming.Infrastructure.ENet;

namespace MocastStudio.Server.SignalStreaming
{
    public sealed class StreamingHubServer : IDisposable
    {
        readonly ISignalStreamingHub _signalStreamingHub;
        readonly ISignalTransportHub _signalTransportHub;

        bool _mainLoopIsRunning;
        bool _signalDispatcherIsRunning;
        bool _transportEventLoopIsRunning;

        public StreamingHubServer(ushort port)
        {
            _signalTransportHub = new ENetTransportHub(port, useAnotherThread: false, targetFrameRate: 0, isBackground: true);
            _signalStreamingHub = new SignalStreamingHub(_signalTransportHub);

            _signalStreamingHub.OnClientConnected += transportClientId =>
            {
                Console.WriteLine($"[{nameof(StreamingHubServer)}] Client connected.");
            };
            _signalStreamingHub.OnClientDisconnected += transportClientId =>
            {
                Console.WriteLine($"[{nameof(StreamingHubServer)}] Client disconnected.");
            };
            _signalStreamingHub.OnDataReceived += OnDataReceived;
        }

        public void Dispose()
        {
            _signalTransportHub.Dispose();
            _signalStreamingHub.Dispose();
            Console.WriteLine($"[{nameof(StreamingHubServer)}] Disposed");
        }

        public void RunTransportEventLoop(CancellationToken cancellationToken)
        {
            if (_transportEventLoopIsRunning)
            {
                throw new InvalidOperationException("Transport event loop is already running.");
            }

            Console.WriteLine($"[{nameof(StreamingHubServer)}] Start transport event loop @Thread:{System.Environment.CurrentManagedThreadId}");

            _transportEventLoopIsRunning = true;
            var spin = new SpinWait();

            while (!cancellationToken.IsCancellationRequested)
            {
                _signalTransportHub.PollEvent();
                spin.SpinOnce();
            }

            _transportEventLoopIsRunning = false;

            Console.WriteLine($"[{nameof(StreamingHubServer)}] End of transport event loop @Thread:{System.Environment.CurrentManagedThreadId}");
        }

        // WIP
        void OnDataReceived(int messageId, uint senderClientId, long originTimestamp, SendOptions sendOptions, ReadOnlyMemory<byte> payload)
        {
            if (messageId == 0)
            {
                var message = MessagePackSerializer.Deserialize<string>(payload);
            }

            if (sendOptions.StreamingType == StreamingType.All)
            {
                _signalStreamingHub.Broadcast(messageId, senderClientId, originTimestamp, payload, sendOptions.Reliable);
            }
        }
    }
}
