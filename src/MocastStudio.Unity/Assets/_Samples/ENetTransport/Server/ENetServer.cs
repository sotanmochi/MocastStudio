
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ENet;
using Event = ENet.Event;
using EventType = ENet.EventType;

namespace SignalTransport.ENet
{
    public sealed class ENetServer : IDisposable
    {
        static readonly double TimestampsToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        readonly Dictionary<uint, Peer> _connectedClients = new();
        readonly int _maxClients = 4000;

        readonly Thread _loopThread;
        readonly CancellationTokenSource _loopCts;
        readonly int _targetFrameTimeMilliseconds;

        byte[] _buffer;
        Host _server;
 
        public event Action<uint> OnConnected;
        public event Action<uint> OnDisconnected;
        public event Action<uint> OnTimeout;
        public event Action<(uint ClientId, ArraySegment<byte> Data)> OnDataReceived;

        public bool IsRunning { get; private set; }

        public ENetServer(ushort port, int targetFrameRate, bool isBackground)
        {
            _buffer = new byte[1024 * 4];

            Library.Initialize();
            _server = new Host();
            _server.Create(new Address(){ Port = port }, _maxClients);

            _targetFrameTimeMilliseconds = (int)(1000 / (double)targetFrameRate);

            _loopCts = new CancellationTokenSource();
            _loopThread = new Thread(RunLoop)
            {
                Name = $"{nameof(ENetServer)}",
                IsBackground = isBackground,
            };

            _loopThread.Start();
        }

        public void Dispose()
        {
            Shutdown();
        }

        public void Shutdown()
        {
            foreach (var peer in _connectedClients.Values)
            {
                peer.DisconnectNow(0);
            }

            _loopCts.Cancel();
            _loopCts.Dispose();

            _server.Flush();
            _server.Dispose();
            _server = null;

            Library.Deinitialize();
        }

        public void Broadcast(ArraySegment<byte> data, bool reliable)
        {
            var flags = reliable
                ? (PacketFlags.Reliable | PacketFlags.NoAllocate) // Reliable Sequenced
                : (PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced

            var packet = default(Packet);
            packet.Create(data.Array, data.Count, flags);

			_server.Broadcast(channelID: 0, ref packet);
        }

        public void Send(uint clientId, ArraySegment<byte> data, bool reliable)
        {
            throw new NotImplementedException();
        }

        void PollEvent()
        {
            var polled = false;

            while (!polled)
            {
                if (_server.CheckEvents(out var netEvent) <= 0)
                {
                    if (_server.Service(0, out netEvent) <= 0) break;
                    polled = true;
                }

                var clientId = netEvent.Peer.ID;

                switch (netEvent.Type)
                {
                    case EventType.None:
                        break;
                    case EventType.Connect:
                        _connectedClients[clientId] = netEvent.Peer;
                        OnConnected?.Invoke(clientId);
                        break;
                    case EventType.Disconnect:
                        _connectedClients.Remove(clientId);
                        OnDisconnected?.Invoke(clientId);
                        break;
                    case EventType.Timeout:
                        _connectedClients.Remove(clientId);
                        OnTimeout?.Invoke(clientId);
                        break;
                    case EventType.Receive:
                        var length = netEvent.Packet.Length;
                        var buffer = (length <= _buffer.Length) ? _buffer : /* Temporary buffer */ new byte[length];
                        Marshal.Copy(netEvent.Packet.Data, buffer, 0, length);
                        netEvent.Packet.Dispose();
                        var data = new ArraySegment<byte>(buffer, 0, length);
                        OnDataReceived?.Invoke((clientId, data)); // Server with server-side logic
                        // Broadcast(data, true); // Relay server
                        break;
                }
            }

            _server.Flush();
        }

        void RunLoop()
        {
            while (!_loopCts.IsCancellationRequested)
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
