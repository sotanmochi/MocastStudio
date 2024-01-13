using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.MotionActor;
using UnityEngine;

namespace MocapSignalTransmission.Transmitter
{
    public sealed class UnityHumanPoseTransmitter : ITransmitter
    {
        private readonly int _id;
        private readonly ISerializer _serializer;
        private readonly ITransport _transport;
        private readonly HashSet<int> _actorIds = new();

        private HumanPose _correctedHumanPose;

        public int Id => _id;
        public uint TransportClientId => _transport.ClientId;
        public IReadOnlyCollection<int> ActorIds => _actorIds;

        public UnityHumanPoseTransmitter(int id, ISerializer serializer, ITransport transport)
        {
            _id = id;
            _serializer = serializer;
            _transport = transport;
            _correctedHumanPose.muscles = new float[HumanTrait.MuscleCount];
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

            // NOTE:
            var worldToLocalMatrix = humanoidMotionActor.ActorSpaceReferenceTransform.worldToLocalMatrix;
            _correctedHumanPose.bodyPosition = worldToLocalMatrix.MultiplyPoint(humanoidMotionActor.HumanPose.bodyPosition);
            _correctedHumanPose.bodyRotation = worldToLocalMatrix.rotation * humanoidMotionActor.HumanPose.bodyRotation;

            for (var i = 0; i < HumanTrait.MuscleCount; i++)
            {
                _correctedHumanPose.muscles[i] = humanoidMotionActor.HumanPose.muscles[i];
            }

            var streamingActorId = TransmitterService.GetStreamingActorId(_transport.ClientId, (byte)humanoidMotionActor.ActorId);

            var serializedData = _serializer.Serialize
            (
                streamingActorId,
                _correctedHumanPose
            );

            _transport.Send(serializedData);
        }
    }
}
