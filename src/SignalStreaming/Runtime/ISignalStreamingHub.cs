using System;
using System.Collections.Generic;

namespace SignalStreaming
{
    public interface ISignalStreamingHub : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public delegate RequestApprovalResult ConnectionRequestValidationFunction(byte[] connectionRequestData);

        /// <summary>
        /// Some message IDs are reserved by the core module of SignalStreaming (ID: 250 ~ 255).
        /// </summary>
        public delegate void OnDataReceivedEventHandler(int messageId, uint senderClientId, long originTimestamp, SendOptions sendOptions, ReadOnlyMemory<byte> payload);

        /// <summary>
        /// 
        /// </summary>
        event ConnectionRequestValidationFunction ConnectionRequestValidationFunc;

        /// <summary>
        /// Some message IDs are reserved by the core module of SignalStreaming (ID: 250 ~ 255).
        /// </summary>
        event OnDataReceivedEventHandler OnDataReceived;

        event Action<uint> OnClientConnected;
        event Action<uint> OnClientDisconnected;
    
        void Send<T>(int messageId, uint senderClientId, long originTimestamp, T data, bool reliable, uint destinationClientId);
        void Send(int messageId, uint senderClientId, long originTimestamp, ReadOnlyMemory<byte> rawMessagePackBlock, bool reliable, uint destinationClientId);
    
        void Broadcast<T>(int messageId, uint senderClientId, long originTimestamp, T data, bool reliable);
        void Broadcast(int messageId, uint senderClientId, long originTimestamp, ReadOnlyMemory<byte> rawMessagePackBlock, bool reliable);
        void Broadcast<T>(int messageId, uint senderClientId, long originTimestamp, T data, bool reliable, IReadOnlyList<uint> destinationClientIds);
        void Broadcast(int messageId, uint senderClientId, long originTimestamp, ReadOnlyMemory<byte> rawMessagePackBlock, bool reliable, IReadOnlyList<uint> destinationClientIds);
    }
}
