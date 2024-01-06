#if ENET_CSHARP

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using ENet;
using Event = ENet.Event;
using EventType = ENet.EventType;
using DebugLogger = SignalStreaming.DevelopmentOnlyLogger;

namespace SignalStreaming.Infrastructure.ENet
{
    /// <summary>
    /// Server implementation of ENet-CSharp
    /// </summary>
    public sealed class ENetTransportHub : ISignalTransportHub
    {
        static readonly double TimestampsToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        readonly Dictionary<uint, Peer> _connectedClients = new();
        readonly int _maxClients = 4000;

        readonly Thread _loopThread;
        readonly CancellationTokenSource _loopCts;
        readonly int _targetFrameTimeMilliseconds;

        byte[] _buffer; // TODO: Ring buffer and dequeue thread
        Host _server;
 
        public event Action<uint> OnConnected;
        public event Action<uint> OnDisconnected;
        public event Action<uint> OnTimeout;
        public event ISignalTransportHub.OnDataReceivedEventHandler OnDataReceived;

        public ENetTransportHub(ushort port, bool useAnotherThread, int targetFrameRate, bool isBackground)
        {
            _buffer = new byte[1024 * 4];

            Library.Initialize();
            _server = new Host();
            _server.Create(new Address(){ Port = port }, _maxClients);

            if (useAnotherThread)
            {
                _targetFrameTimeMilliseconds = (int)(1000 / (double)targetFrameRate);

                _loopCts = new CancellationTokenSource();
                _loopThread = new Thread(RunLoop)
                {
                    Name = $"{nameof(ENetTransportHub)}",
                    IsBackground = isBackground,
                };

                // TODO: Ring buffer and dequeue thread
            }
        }

        public void Dispose()
        {
            Shutdown();
            DebugLogger.Log($"[{nameof(ENetTransportHub)}] Disposed.");
        }

        public void Start()
        {
            _loopThread.Start();
        }

        public void Shutdown()
        {
            DebugLogger.Log($"[{nameof(ENetTransportHub)}] Shutdown...");

            DisconnectAll();

            _loopCts?.Cancel();
            _loopCts?.Dispose();

            _server.Flush();
            _server.Dispose();
            _server = null;

            Library.Deinitialize();
        }

        public void Disconnect(uint clientId)
        {
            if (_connectedClients.Remove(clientId, out var peer))
            {
                // Requests a disconnection from a peer,
                // but only after all queued outgoing packets are sent.
                peer.DisconnectLater(0);
            }
        }

        public void DisconnectAll()
        {
            foreach (var peer in _connectedClients.Values)
            {
                // Requests a disconnection from a peer,
                // but only after all queued outgoing packets are sent.
                peer.DisconnectLater(0);
            }
            _connectedClients.Clear();
        }

        public void Send(ArraySegment<byte> data, bool reliable, uint destinationClientId)
        {
            if (_connectedClients.TryGetValue(destinationClientId, out var peer))
            {
                var flags = reliable
                    ? (PacketFlags.Reliable | PacketFlags.NoAllocate) // Reliable Sequenced
                    : (PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced

                var packet = default(Packet);
                packet.Create(data.Array, data.Count, flags);

                peer.Send(channelID: 0, ref packet);
            }
        }

        public void Broadcast(ArraySegment<byte> data, bool reliable, IReadOnlyList<uint> destinationClientIds)
        {
            var flags = reliable
                ? (PacketFlags.Reliable | PacketFlags.NoAllocate) // Reliable Sequenced
                : (PacketFlags.None | PacketFlags.NoAllocate); // Unreliable Sequenced

            var packet = default(Packet);
            packet.Create(data.Array, data.Count, flags);

            var peers = _connectedClients
                .Select(x => x.Value)
                .Where(peer => destinationClientIds.Contains(GetClientId(peer)))
                .ToArray();

            _server.Broadcast(channelID: 0, ref packet, peers);
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

        // TODO: Ring buffer and dequeue thread

        public void PollEvent()
        {
            var polled = false;

            while (!polled)
            {
                if (_server.CheckEvents(out var netEvent) <= 0)
                {
                    if (_server.Service(0, out netEvent) <= 0) break;
                    polled = true;
                }

                var clientId = GetClientId(netEvent.Peer);

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
                        // REVIEW: Timeout event handling
                        break;
                    case EventType.Receive:
                        var length = netEvent.Packet.Length;
                        var buffer = (length <= _buffer.Length) ? _buffer : /* Temporary buffer */ new byte[length];
                        Marshal.Copy(netEvent.Packet.Data, buffer, 0, length);
                        netEvent.Packet.Dispose();
                        // TODO: Ring buffer and dequeue thread
                        OnDataReceived?.Invoke(clientId, new ArraySegment<byte>(buffer, 0, length));
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
        
        uint GetClientId(Peer peer) => peer.ID + 1;
    }
}

#endif