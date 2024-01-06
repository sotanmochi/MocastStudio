using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.Transmitter;

namespace MocapSignalTransmission.Infrastructure.Transmitter.Transport
{
    public sealed class UdpTransport : ITransport
    {
        private readonly string _serverHost;
        private readonly int _serverPort;

        private UdpClient _client;
        private bool _connected;
        private bool _connecting;

        public string ServerHost => _serverHost;
        public int ServerPort => _serverPort;

        public bool IsConnected => _connected;
        public bool IsConnecting => _connecting;
        public uint ClientId => 0;

        public UdpTransport(string serverHost, int serverPort)
        {
            _serverHost = serverHost;
            _serverPort = serverPort;
        }

        public void Dispose() => Disconnect();

        public async Task DisconnectAsync() => Disconnect();

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_connecting || _connected) return _connected;
            _connecting = true;

            try
            {
                _client = new UdpClient();
                _client.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
                _client.Connect(_serverHost, _serverPort);

                _connected = true;
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

        public void Send(ReadOnlySequence<byte> buffer)
        {
            if (_connected)
            {
                var byteArray = buffer.ToArray();
                _client.Send(byteArray, byteArray.Length);
            }
        }

        private void Disconnect()
        {
            _client.Close();
            _client = null;
            _connected = false;
            _connecting = false;
        }
    }
}
