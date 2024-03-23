using System;
using System.Buffers;
using System.Threading;
using Cysharp.Threading.Tasks;
using SignalStreaming;
using SignalStreaming.Infrastructure.LiteNetLib;
using R3;

namespace MocastStudio.Application.OnlineStudio
{
    public sealed class ConnectionService : IDisposable
    {
        readonly ConnectionServiceContext _context;
        readonly IOnlineStudioEndpointProvider _endpointProvider;
        readonly ISignalStreamingClient _streamingClient;
        readonly LiteNetLibConnectParameters _connectionParameters = new();

        public ConnectionService(
            ConnectionServiceContext context,
            IOnlineStudioEndpointProvider endpointProvider,
            ISignalStreamingClient streamingClient)
        {
            _context = context;
            _endpointProvider = endpointProvider;
            _streamingClient = streamingClient;
            _streamingClient.OnConnected += OnConnected;
            _streamingClient.OnDisconnected += OnDisconnected;
            _streamingClient.OnIncomingSignalDequeued += OnIncomingSignalDequeued;
        }

        public void Dispose()
        {
            DisconnectAsync().GetAwaiter().GetResult();
            _streamingClient.OnConnected -= OnConnected;
            _streamingClient.OnDisconnected -= OnDisconnected;
            _streamingClient.OnIncomingSignalDequeued -= OnIncomingSignalDequeued;
            Log($"Disposed");
        }

        public void UpdateConnectionRequestData(byte[] data)
        {
            _connectionParameters.ConnectionRequestData = data;
        }

        public async UniTask<bool> JoinAsync(string groupId, CancellationToken cancellationToken)
        {
            if (!_context.connected.Value)
            {
                var endpoint = _endpointProvider.FindBySignalStreamingGroupIdAsync(groupId, cancellationToken);
                _connectionParameters.ServerAddress = endpoint.Address;
                _connectionParameters.ServerPort = endpoint.Port;

                Log($"Trying to connect to server... (Thread: {Thread.CurrentThread.ManagedThreadId})");
                var connected = await _streamingClient.ConnectAsync(_connectionParameters, cancellationToken);
                if (!connected)
                {
                    Log($"Failed to connect. (Thread: {Thread.CurrentThread.ManagedThreadId})");
                    _context.connected.Value = false;
                    _context.joined.Value = false;
                    return false;
                }
                Log($"Connected to server. (Thread: {Thread.CurrentThread.ManagedThreadId})");
            }

            Log($"Trying to join group... (Thread: {Thread.CurrentThread.ManagedThreadId})");
            _context.GroupId = groupId;
            _context.joined.Value = await _streamingClient.JoinGroupAsync(_context.GroupId, cancellationToken);

            if (_context.joined.Value)
            {
                Log($"Joined group: {_context.GroupId} (Thread: {Thread.CurrentThread.ManagedThreadId})");
            }
            else
            {
                Log($"Failed to join group: {_context.GroupId} (Thread: {Thread.CurrentThread.ManagedThreadId})");
            }

            return _context.joined.Value;
        }

        public async UniTask LeaveAsync(string groupId, CancellationToken cancellationToken)
        {
            await _streamingClient.LeaveGroupAsync(groupId, cancellationToken);
            _context.joined.Value = false;
        }

        public async UniTask DisconnectAsync()
        {
            await _streamingClient.DisconnectAsync();
            Log($"Disconnected from server. (Thread: {Thread.CurrentThread.ManagedThreadId})");
        }

        void OnConnected(uint streamingClientId)
        {
            Log($"Connected - ClientId: {streamingClientId}");
            _context.streamingClientId = streamingClientId;
        }

        void OnDisconnected(string reason)
        {
            Log($"Disconnected - Reason: {reason}");
            _context.streamingClientId = 0;
        }

        void OnIncomingSignalDequeued(int signalId, ReadOnlySequence<byte> byteSequence, uint senderClientId)
        {
            // TODO
        }

        void Log(object message)
        {
            UnityEngine.Debug.Log($"[{nameof(ConnectionService)}] {message}");
        }
    }
}
