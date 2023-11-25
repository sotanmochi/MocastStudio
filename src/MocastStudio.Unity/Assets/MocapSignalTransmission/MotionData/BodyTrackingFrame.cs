using UnityEngine;

namespace MocapSignalTransmission.MotionData
{
    public sealed class BodyTrackingFrame
    {
        public static int BoneCount => (int)BodyTrackingBones.Count;

        public Vector3 Scale { get; set; }

        public Vector3 RootPosition { get; set; }
        public Quaternion RootRotation { get; set; }

        public Vector3[] BonePositions { get; }
        public Quaternion[] BoneRotations { get; }

        public BodyTrackingFrame()
        {
            Scale = Vector3.one;
            RootPosition = Vector3.zero;
            RootRotation = Quaternion.identity;
            BonePositions = new Vector3[(int)BodyTrackingBones.Count];
            BoneRotations = new Quaternion[(int)BodyTrackingBones.Count];
        }
    }
}
