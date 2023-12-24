using System;
using System.Collections.Generic;

namespace SignalStreaming
{
    public interface ISignalTransportHub : IDisposable
    {
        public delegate void OnDataReceivedEventHandler(uint senderClientId, ArraySegment<byte> data);

        event Action<uint> OnConnected;
        event Action<uint> OnDisconnected;
        event OnDataReceivedEventHandler OnDataReceived;

        void Start();
        void Shutdown();

        void Disconnect(uint clientId);
        void DisconnectAll();

        void Send(ArraySegment<byte> data, bool reliable, uint destinationClientId);
        void Broadcast(ArraySegment<byte> data, bool reliable, IReadOnlyList<uint> destinationClientIds);
        void Broadcast(ArraySegment<byte> data, bool reliable);
    }
}
