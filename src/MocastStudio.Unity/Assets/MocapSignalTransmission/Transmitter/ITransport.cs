using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace MocapSignalTransmission.Transmitter
{
    public interface ITransport : IDisposable
    {
        bool IsConnected { get; }
        uint ClientId { get; }
        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        void Send(ReadOnlySequence<byte> buffer);
    }
}
