using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.MotionActor;

namespace MocapSignalTransmission.Transmitter
{
    public interface ITransmitter
    {
        uint TransportClientId { get; }
        IReadOnlyCollection<int> ActorIds { get; }
        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        void AddActorId(int value);
        void RemoveActorId(int value);
        void Send<T>(T data) where T : IMotionActor;
    }
}
