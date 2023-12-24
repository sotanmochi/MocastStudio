using System;
using System.Collections.Generic;
using MessagePack;
using DebugLogger = SignalStreaming.DevelopmentOnlyLogger;

namespace SignalStreaming
{
    public class SignalStreamingHub : IDisposable
    {
        /// <summary>
        /// Some message IDs are reserved by the core module of SignalStreaming. (ID: 250 ~ 255)
        /// </summary>
        public delegate void OnDataReceivedEventHandler(int messageId, uint senderClientId, long originTimestamp, SendOptions sendOptions, ReadOnlyMemory<byte> payload);

        public delegate RequestApprovalResult ConnectionRequestValidationFunction(byte[] connectionRequestData);

        ISignalTransportHub _transportHub;

        public event ConnectionRequestValidationFunction ConnectionRequestValidationFunc;
        public event Action<uint> OnClientConnected;
        public event Action<uint> OnClientDisconnected;

        /// <summary>
        /// Some message IDs are reserved by the core module of SignalStreaming. (ID: 250 ~ 255)
        /// </summary>
        public event OnDataReceivedEventHandler OnDataReceived;

        public SignalStreamingHub(ISignalTransportHub transportHub)
        {
            _transportHub = transportHub;
            _transportHub.OnConnected += OnTransportConnected;
            _transportHub.OnDisconnected += OnTransportDisconnected;
            _transportHub.OnDataReceived += OnDataReceivedInternal;
        }

        public void Dispose()
        {
            _transportHub.OnConnected -= OnTransportConnected;
            _transportHub.OnDisconnected -= OnTransportDisconnected;
            _transportHub.OnDataReceived -= OnDataReceivedInternal;
            _transportHub = null;
        }

        public void Initialize()
        {
            _transportHub.Start();
        }

        public void Send<T>(int messageId, uint senderClientId, long originTimestamp, T data, bool reliable, uint destinationClientId)
        {
            var transmitTimestamp = TimestampProvider.GetCurrentTimestamp();
            var serializedMessage = Serialize(messageId, senderClientId, originTimestamp, transmitTimestamp, data);
            _transportHub.Send(serializedMessage, reliable, destinationClientId);
        }

        public void Send(int messageId, uint senderClientId, long originTimestamp, ReadOnlyMemory<byte> rawMessagePackBlock, bool reliable, uint destinationClientId)
        {
            var transmitTimestamp = TimestampProvider.GetCurrentTimestamp();
            var serializedMessage = Serialize(messageId, senderClientId, originTimestamp, transmitTimestamp, rawMessagePackBlock);
            _transportHub.Send(serializedMessage, reliable, destinationClientId);
        }

        public void Broadcast<T>(int messageId, uint senderClientId, long originTimestamp, T data, bool reliable)
        {
            var transmitTimestamp = TimestampProvider.GetCurrentTimestamp();
            var serializedMessage = Serialize(messageId, senderClientId, originTimestamp, transmitTimestamp, data);
            _transportHub.Broadcast(serializedMessage, reliable);
        }

        public void Broadcast(int messageId, uint senderClientId, long originTimestamp, ReadOnlyMemory<byte> rawMessagePackBlock, bool reliable)
        {
            var transmitTimestamp = TimestampProvider.GetCurrentTimestamp();
            var serializedMessage = Serialize(messageId, senderClientId, originTimestamp, transmitTimestamp, rawMessagePackBlock);
            _transportHub.Broadcast(serializedMessage, reliable);
        }

        public void Broadcast<T>(int messageId, uint senderClientId, long originTimestamp, T data, bool reliable, IReadOnlyList<uint> destinationClientIds)
        {
            var transmitTimestamp = TimestampProvider.GetCurrentTimestamp();
            var serializedMessage = Serialize(messageId, senderClientId, originTimestamp, transmitTimestamp, data);
            _transportHub.Broadcast(serializedMessage, reliable, destinationClientIds);
        }

        public void Broadcast(int messageId, uint senderClientId, long originTimestamp, ReadOnlyMemory<byte> rawMessagePackBlock, bool reliable, IReadOnlyList<uint> destinationClientIds)
        {
            var transmitTimestamp = TimestampProvider.GetCurrentTimestamp();
            var serializedMessage = Serialize(messageId, senderClientId, originTimestamp, transmitTimestamp, rawMessagePackBlock);
            _transportHub.Broadcast(serializedMessage, reliable, destinationClientIds);
        }

        void OnTransportConnected(uint clientId)
        {
            var messageId = (int)MessageType.TransportConnected;
            var transmitTimestamp = TimestampProvider.GetCurrentTimestamp();
            var connectionMessage = SerializeConnectionMessage(
                messageId, originTimestamp: transmitTimestamp, transmitTimestamp, clientId, "ConnectedToHubServer");
            _transportHub.Send(connectionMessage, reliable: true, clientId);
        }

        void OnTransportDisconnected(uint clientId)
        {
            OnClientDisconnected?.Invoke(clientId);
        }

        void OnDataReceivedInternal(uint clientId, ArraySegment<byte> data)
        {
            var reader = new MessagePackReader(data);

            var arrayLength = reader.ReadArrayHeader();
            if (arrayLength != 5)
            {
                throw new InvalidOperationException($"[{nameof(SignalStreamingHub)}] Invalid data format.");
            }

            var messageId = reader.ReadInt32();
            var senderClientId = reader.ReadUInt32();
            var originTimestamp = reader.ReadInt64();
            var sendOptions = MessagePackSerializer.Deserialize<SendOptions>(ref reader);

            var payloadOffset = data.Offset + (int)reader.Consumed;
            var payloadCount = data.Count - (int)reader.Consumed;
            var payload = new ReadOnlyMemory<byte>(data.Array, payloadOffset, payloadCount);

            if (messageId == (int)MessageType.ClientConnectionRequest)
            {
                var connectionRequestData = MessagePackSerializer.Deserialize<byte[]>(payload);

                var result = ConnectionRequestValidationFunc != null
                    ? ConnectionRequestValidationFunc.Invoke(connectionRequestData)
                    : new RequestApprovalResult(approved: true, message: "No connection request validation.");

                var transmitTimestamp = TimestampProvider.GetCurrentTimestamp();
                var responseMessage = SerializeConnectionMessage((int)MessageType.ClientConnectionResponse,
                    originTimestamp: transmitTimestamp, transmitTimestamp, senderClientId, result);

                _transportHub.Send(responseMessage, reliable: true, destinationClientId: senderClientId);

                if (result.Approved)
                {
                    OnClientConnected?.Invoke(senderClientId);
                }
                else
                {
                    _transportHub.Disconnect(senderClientId);
                }
            }
            else
            {
                OnDataReceived?.Invoke(messageId, senderClientId, originTimestamp, sendOptions, payload);
            }
        }

        byte[] Serialize(int messageId, uint senderClientId, long originTimestamp, long transmitTimestamp, ReadOnlyMemory<byte> rawMessagePackBlock)
        {
            using var bufferWriter = ArrayPoolBufferWriter.RentThreadStaticWriter();
            var writer = new MessagePackWriter(bufferWriter);
            writer.WriteArrayHeader(6);
            writer.Write(messageId);
            writer.Write(senderClientId);
            writer.Write(originTimestamp);
            writer.Write(transmitTimestamp);
            // writer.Write(0);
            writer.WriteRaw(rawMessagePackBlock.Span); // NOTE
            writer.Flush();
            return bufferWriter.WrittenSpan.ToArray();
        }

        byte[] Serialize<T>(int messageId, uint senderClientId, long originTimestamp, long transmitTimestamp, T data)
        {
            using var bufferWriter = ArrayPoolBufferWriter.RentThreadStaticWriter();
            var writer = new MessagePackWriter(bufferWriter);
            writer.WriteArrayHeader(6);
            writer.Write(messageId);
            writer.Write(senderClientId);
            writer.Write(originTimestamp);
            writer.Write(transmitTimestamp);
            // writer.Write(0);
            writer.Flush();
            MessagePackSerializer.Serialize(bufferWriter, data);
            return bufferWriter.WrittenSpan.ToArray();
        }

        byte[] SerializeConnectionMessage<T>(int messageId, long originTimestamp, long transmitTimestamp, uint connectingClientId, T data)
        {
            using var bufferWriter = ArrayPoolBufferWriter.RentThreadStaticWriter();
            var writer = new MessagePackWriter(bufferWriter);
            writer.WriteArrayHeader(6);
            writer.Write(messageId);
            writer.Write(0); // NOTE: The sender client ID is set to 0, because this message is sent from the hub server.
            writer.Write(originTimestamp);
            writer.Write(transmitTimestamp);
            writer.Write(connectingClientId);
            writer.Flush();
            MessagePackSerializer.Serialize(bufferWriter, data);
            return bufferWriter.WrittenSpan.ToArray();
        }
    }
}
