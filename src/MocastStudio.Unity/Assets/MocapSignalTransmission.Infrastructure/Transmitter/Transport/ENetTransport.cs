#if ENET_CSHARP

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ENet;
using Event = ENet.Event;
using EventType = ENet.EventType;
using MocapSignalTransmission.Transmitter;

namespace MocapSignalTransmission.Infrastructure.Transmitter.Transport
{
    /// <summary>
    /// Client implementation of ENet-CSharp
    /// </summary>
    public sealed class ENetTransport : ITransport
    {
        static readonly double TimestampsToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        readonly Thread _receiverLoopThread;
        readonly CancellationTokenSource _receiverLoopCts;
        readonly int _targetFrameTimeMilliseconds;

        string _serverAddress;
        ushort _serverPort;
        bool _reliable;
        byte[] _buffer;

        Address _address;
        Host _client;
        Peer _peer;

        TaskCompletionSource<bool> _connectionTcs;
        bool _connected;
        bool _connecting;

        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action OnTimeout;
        public event Action<ArraySegment<byte>> OnDataReceived;

        public bool IsConnected => _connected;

        public ENetTransport(
            string serverAddress, ushort serverPort, bool reliable,
            bool useAnotherThread, int targetFrameRate, bool isBackground)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
            _reliable = reliable;
            _buffer = new byte[1024 * 4];

            Library.Initialize();
            _address = new Address();
            _client = new Host();
            _client.Create();

            if (useAnotherThread)
            {
                _targetFrameTimeMilliseconds = (int)(1000 / (double)targetFrameRate);

                _receiverLoopCts = new CancellationTokenSource();
                _receiverLoopThread = new Thread(ReceiverLoop)
                {
                    Name = $"{nameof(ENetTransport)}",
                    IsBackground = isBackground,
                };

                _receiverLoopThread.Start();
            }
        }

        public void Dispose()
        {
            Disconnect();

            _receiverLoopCts?.Cancel();
            _receiverLoopCts?.Dispose();

            _client.Flush();
            _client.Dispose();
            _client = null;

            Library.Deinitialize();
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_connecting || _connected) return _connected;
            _connecting = true;
            _connectionTcs = new TaskCompletionSource<bool>();

            try
            {
                _address.SetHost(_serverAddress);
                _address.Port = _serverPort;
                _peer = _client.Connect(_address, channelLimit: 4);

                _connected = await _connectionTcs.Task;
                _connecting = false;
            }
            catch (Exception e)
            {
                _connected = false;
                _connecting = false;
                throw;
            }

            return _connected;
        }

        public async Task DisconnectAsync() => Disconnect();

        public void Disconnect()
        {
            if (_connected)
            {
                _peer.DisconnectNow(0);
                _connected = false;
                _connecting = false;
            }
        }

        public void Send(ArraySegment<byte> data)
        {
            var flags = _reliable
                ? (PacketFlags.Reliable | PacketFlags.NoAllocate) // Reliable Sequenced
                : (PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced

            var packet = default(Packet);
            packet.Create(data.Array, data.Count, flags);

            _peer.Send(channelID: 0, ref packet);
        }

        public void PollEvent()
        {
            var polled = false;

            while (!polled)
            {
                if (_client.CheckEvents(out var netEvent) <= 0)
                {
                    if (_client.Service(0, out netEvent) <= 0) break;
                    polled = true;
                }

                switch (netEvent.Type)
                {
                    case EventType.None:
                        break;
                    case EventType.Connect:
                        _connectionTcs.SetResult(true);
                        OnConnected?.Invoke();
                        break;
                    case EventType.Disconnect:
                        OnDisconnected?.Invoke();
                        break;
                    case EventType.Timeout:
                        OnTimeout?.Invoke();
                        break;
                    case EventType.Receive:
                        var length = netEvent.Packet.Length;
                        var buffer = (length <= _buffer.Length) ? _buffer : /* Temporary buffer */ new byte[length];
                        Marshal.Copy(netEvent.Packet.Data, buffer, 0, length);
                        netEvent.Packet.Dispose();
                        OnDataReceived?.Invoke(new ArraySegment<byte>(buffer, 0, length));
                        break;
                }
            }

            _client.Flush();
        }

        void ReceiverLoop()
        {
            while (!_receiverLoopCts.IsCancellationRequested)
            {
                var begin = Stopwatch.GetTimestamp();

                PollEvent();

                var end = Stopwatch.GetTimestamp();
                var elapsedTicks = (end - begin) * TimestampsToTicks;
                var elapsedMilliseconds = (long)elapsedTicks / TimeSpan.TicksPerMillisecond;

                var waitForNextFrameMilliseconds = (int)(_targetFrameTimeMilliseconds - elapsedMilliseconds);
                if (waitForNextFrameMilliseconds > 0)
                {
                    Thread.Sleep(waitForNextFrameMilliseconds);
                }
            }
        }
    }
}

#endif