#if SIGNAL_STREAMING

using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.Transmitter;
using SignalStreaming;

namespace MocapSignalTransmission.Infrastructure.Transmitter.Transport
{
    public sealed class SignalStreamingTransportAdapter : ITransport
    {
        ISignalStreamingClient _signalStreamingClient;
        IConnectParameters _connectParameters;
        SendOptions _sendOptions;
        int _messageId;

        public bool IsConnected => _signalStreamingClient.IsConnected;

        public SignalStreamingTransportAdapter(
            ISignalStreamingClient signalStreamingClient, IConnectParameters connectParameters, int messageId)
        {
            _signalStreamingClient = signalStreamingClient;
            _connectParameters = connectParameters;
            _messageId = messageId;
            _sendOptions = new SendOptions(StreamingType.All, reliable: true);
            // _sendOptions = new SendOptions(StreamingType.ExceptSelf, reliable: true);
        }

        public void Dispose()
        {
            _signalStreamingClient.Dispose();
            _signalStreamingClient = null;
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
            => await _signalStreamingClient.ConnectAsync(_connectParameters, cancellationToken);

        public async Task DisconnectAsync()
            => await _signalStreamingClient.DisconnectAsync();

        public void Send(ReadOnlySequence<byte> buffer)
        {
            if (!_signalStreamingClient.IsConnected) return;
            _signalStreamingClient.Send(_messageId, buffer, _sendOptions);
        }
    }
}

#endif