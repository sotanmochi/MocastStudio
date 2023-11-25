using UnityEngine;

namespace MocapSignalTransmission.MotionData
{
    public sealed class FingerTrackingFrame
    {
        public static int BoneCount => (int)FingerTrackingBones.Count;

        public Quaternion[] BoneRotations { get; }

        public FingerTrackingFrame()
        {
            BoneRotations = new Quaternion[(int)FingerTrackingBones.Count];
        }
    }
}
