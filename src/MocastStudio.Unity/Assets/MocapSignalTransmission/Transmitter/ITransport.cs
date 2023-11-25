using System;
using System.Threading;
using System.Threading.Tasks;

namespace MocapSignalTransmission.Transmitter
{
    public interface ITransport : IDisposable
    {
        bool IsConnected { get; }
        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        void Send(ArraySegment<byte> data);
    }
}
