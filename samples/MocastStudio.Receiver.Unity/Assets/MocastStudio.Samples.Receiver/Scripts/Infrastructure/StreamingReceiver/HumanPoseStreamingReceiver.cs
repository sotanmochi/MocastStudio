using System;
using MessagePipe;
using UnityEngine;
using uOSC;

namespace MocastStudio.Samples.Receiver.Infrastructure.StreamingReceiver
{
    public sealed class HumanPoseStreamingReceiver : IDisposable
    {
        // HumanPoseValueCount = ActorId (int x 1)
        //                     + HumanPose.bodyPosition (float x 3)
        //                     + HumanPose.bodyRotation (float x 4)
        //                     + HumanPose.muscles (float x 95)
        // (Refer to HumanPoseOscSerializer.cs)
        public static readonly int HumanPoseValueCount = 103;

        private readonly IPublisher<HumanPose> _humanPoseUpdateEventPublisher;

        private uOscServer _oscServer;

        private string _oscMessageAddress = "/HumanPose";
        private int _filterActorId;
        private HumanPose _humanPose;

        public string OscMessageAddress => _oscMessageAddress;

        public HumanPoseStreamingReceiver(uOscServer oscServer, IPublisher<HumanPose> humanPoseUpdateEventPublisher)
        {
            _oscServer = oscServer;
            _humanPoseUpdateEventPublisher = humanPoseUpdateEventPublisher;
            _humanPose.muscles = new float[HumanTrait.MuscleCount];
            _oscServer.onDataReceived.AddListener(OnDataReceived);
        }

        public void Dispose()
        {
            _oscServer.onDataReceived.RemoveListener(OnDataReceived);
            _oscServer = null;
        }

        public void Start()
        {
            _oscServer.StartServer();
        }

        public void Stop()
        {
            _oscServer.StopServer();
        }

        public void UpdatePort(int port)
        {
            _oscServer.port = port;
        }

        public void UpdateMessageAddress(string address)
        {
            _oscMessageAddress = address;
        }

        public void UpdateActorIdFilter(int actorId)
        {
            _filterActorId = actorId;
        }

        private void OnDataReceived(Message message)
        {
            if (message.address == OscMessageAddress && message.values.Length == HumanPoseValueCount)
            {
                var actorId = (int)message.values[0];

                if (_filterActorId != actorId) return;

                var bodyPositionX = (float)message.values[1];
                var bodyPositionY = (float)message.values[2];
                var bodyPositionZ = (float)message.values[3];
                var bodyRotationX = (float)message.values[4];
                var bodyRotationY = (float)message.values[5];
                var bodyRotationZ = (float)message.values[6];
                var bodyRotationW = (float)message.values[7];
                var messageOffset = 8;

                _humanPose.bodyPosition = new Vector3(bodyPositionX, bodyPositionY, bodyPositionZ);
                _humanPose.bodyRotation = new Quaternion(bodyRotationX, bodyRotationY, bodyRotationZ, bodyRotationW);

                for (var i = 0; i < _humanPose.muscles.Length; i++)
                {
                    _humanPose.muscles[i] = (float)message.values[i + messageOffset];
                }

                _humanPoseUpdateEventPublisher.Publish(_humanPose);
            }
            else
            {
                Debug.LogError($"[{nameof(HumanPoseStreamingReceiver)}] Unknown message: {message.address}");
            }
        }
    }
}
