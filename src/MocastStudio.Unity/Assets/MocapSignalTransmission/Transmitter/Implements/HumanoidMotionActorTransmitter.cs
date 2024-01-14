using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.MotionActor;
using Debug = UnityEngine.Debug;

namespace MocapSignalTransmission.Transmitter
{
    public sealed class HumanoidMotionActorTransmitter : ITransmitter
    {
        private readonly int _id;
        private readonly ISerializer _serializer;
        private readonly ITransport _transport;
        private readonly HashSet<int> _actorIds = new();

        public int Id => _id;
        public uint TransportClientId => _transport.ClientId;
        public IReadOnlyCollection<int> ActorIds => _actorIds;

        public HumanoidMotionActorTransmitter(int id, ISerializer serializer, ITransport transport)
        {
            _id = id;
            _serializer = serializer;
            _transport = transport;
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            return await _transport.ConnectAsync(cancellationToken);
        }

        public async Task DisconnectAsync()
        {
            await _transport.DisconnectAsync();
        }

        public void AddActorId(int value)
        {
            _actorIds.Add(value);
        }

        public void RemoveActorId(int value)
        {
            _actorIds.Remove(value);
        }

        public void Send<T>(T data) where T : IMotionActor
        {
            if (!_transport.IsConnected || !_actorIds.Contains(data.ActorId))
            {
                return;
            }

            var humanoidMotionActor = data as HumanoidMotionActor;
            if (humanoidMotionActor == null)
            {
                Debug.LogError($"Invalid data type for {nameof(UnityHumanPoseTransmitter)}. ActorId: {data.ActorId}, Name: {data.Name}");
                return;
            }

            var streamingActorId = TransmitterService.GetStreamingActorId(_transport.ClientId, humanoidMotionActor.ActorId);

            var serializedData = _serializer.Serialize
            (
                streamingActorId,
                humanoidMotionActor
            );

            _transport.Send(serializedData);
        }
    }
}
