using System;
using MessagePack;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace MocapSignalTransmission.MotionData
{
    [MessagePackObject]
    public struct ActorHumanPose
    {
        [Key(0)]
        public int ActorId { get; set; }

        [Key(1)]
        public Vector3 BodyPosition { get; set; }

        [Key(2)]
        public Quaternion BodyRotation { get; set; }

        [Key(3)]
        public float[] Muscles { get; set; }
    }
}
