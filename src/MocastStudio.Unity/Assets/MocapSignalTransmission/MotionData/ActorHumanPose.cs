using System;
using MessagePack;
using HumanPose = UnityEngine.HumanPose;

namespace MocapSignalTransmission.MotionData
{
    [MessagePackObject]
    public struct ActorHumanPose
    {
        [Key(0)]
        public int ActorId { get; set; }

        [Key(1)]
        public HumanPose HumanPose { get; set; }
    }
}
