using MocapSignalTransmission.MotionData;
using MocapSignalTransmission.MotionDataSource;
using UnityEngine;
using UnityEngine.Assertions;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class VMCProtocolDataBuffer : IBodyTrackingDataSource, IFingerTrackingDataSource
    {
        private readonly BodyTrackingFrame[] _bodyTrackingFrameBuffer;
        private readonly FingerTrackingFrame[] _fingerTrackingFrameBuffer;
        private readonly int _bufferSize;
        private readonly long _bufferMask;

        private long _bufferHead = 0;
        private long _bufferTail = 0;

        public int Id { get; }

        public VMCProtocolDataBuffer(int id, int bufferSize = 2)
        {
            Assert.IsTrue(Utils.IsPowerOfTwo(bufferSize), "The buffer size must be a power of two.");
            _bufferSize = bufferSize;
            _bufferMask = bufferSize - 1;

            Id = id;

            _bodyTrackingFrameBuffer = new BodyTrackingFrame[bufferSize];
            _fingerTrackingFrameBuffer = new FingerTrackingFrame[bufferSize];

            for (var i = 0; i < bufferSize; i++)
            {
                _bodyTrackingFrameBuffer[i] = new BodyTrackingFrame();
                _fingerTrackingFrameBuffer[i] = new FingerTrackingFrame();
            }
        }

        /// <summary>
        /// Try to get a reference of the first data in the buffer.
        /// The data is not deleted from the buffer in this method.
        /// When the buffer overflows during enqueue, the first data (oldest data) is automatically deleted.
        /// </summary>
        /// <param name="bodyTrackingFrame"></param>
        /// <returns></returns>
        public bool TryPeek(out BodyTrackingFrame bodyTrackingFrame)
        {
            var bufferIndex = _bufferHead & _bufferMask;
            bodyTrackingFrame = _bodyTrackingFrameBuffer[bufferIndex];
            return true;
        }

        /// <summary>
        /// Try to get a reference of the first data in the buffer.
        /// The data is not deleted from the buffer in this method.
        /// When the buffer overflows during enqueue, the first data (oldest data) is automatically deleted.
        /// </summary>
        /// <param name="fingerTrackingFrame"></param>
        /// <returns></returns>
        public bool TryPeek(out FingerTrackingFrame fingerTrackingFrame)
        {
            var bufferIndex = _bufferHead & _bufferMask;
            fingerTrackingFrame = _fingerTrackingFrameBuffer[bufferIndex];
            return true;
        }

        public void Enqueue(VMCProtocolStreamingReceiver.SkeletonData skeletonData)
        {
            // Update the enqueue position to insert the next data.
            _bufferTail++;
            var enqueueIndex = _bufferTail & _bufferMask;

            // When the buffer overflows, the head data (oldest data) is deleted.
            var bufferFreeCount = _bufferSize - (int) (_bufferTail - _bufferHead);
            if (bufferFreeCount <= 0)
            {
                _bufferHead++;
            }

            _bodyTrackingFrameBuffer[enqueueIndex].RootPosition = skeletonData.RootBonePosition;
            _bodyTrackingFrameBuffer[enqueueIndex].RootRotation = skeletonData.RootBoneRotation;
            _bodyTrackingFrameBuffer[enqueueIndex].Scale = skeletonData.Scale;

            var boneCount = skeletonData.BoneRotations.Length;
            for (var i = 0; i < boneCount; i++)
            {
                var bodyBoneId = GetBodyTrackingBoneId(i);
                var fingerBoneId = GetFingerTrackingBoneId(i);

                if (bodyBoneId < 0 && fingerBoneId < 0) continue;

                if (bodyBoneId >= 0)
                {
                    // _bodyTrackingFrameBuffer[enqueueIndex].BonePositions[bodyBoneId] = skeletonData.BonePositions[i];
                    _bodyTrackingFrameBuffer[enqueueIndex].BoneRotations[bodyBoneId] = skeletonData.BoneRotations[i];
                }

                if (fingerBoneId >= 0)
                {
                    // _fingerTrackingFrameBuffer[enqueueIndex].BonePositions[fingerBoneId] = skeletonData.BonePositions[i];
                    _fingerTrackingFrameBuffer[enqueueIndex].BoneRotations[fingerBoneId] = skeletonData.BoneRotations[i];
                }
            }
        }

        public static int GetBodyTrackingBoneId(int humanBodyBoneId)
        {
            var bodyTrackingBoneId = humanBodyBoneId switch
            {
                (int)HumanBodyBones.Hips          => (int)BodyTrackingBones.Hips,
                (int)HumanBodyBones.Spine         => (int)BodyTrackingBones.Spine,
                (int)HumanBodyBones.Chest         => (int)BodyTrackingBones.Chest,
                (int)HumanBodyBones.UpperChest    => (int)BodyTrackingBones.UpperChest,
                (int)HumanBodyBones.Neck          => (int)BodyTrackingBones.Neck,
                (int)HumanBodyBones.Head          => (int)BodyTrackingBones.Head,
                (int)HumanBodyBones.LeftShoulder  => (int)BodyTrackingBones.LeftShoulder,
                (int)HumanBodyBones.LeftUpperArm  => (int)BodyTrackingBones.LeftUpperArm,
                (int)HumanBodyBones.LeftLowerArm  => (int)BodyTrackingBones.LeftLowerArm,
                (int)HumanBodyBones.LeftHand      => (int)BodyTrackingBones.LeftHand,
                (int)HumanBodyBones.RightShoulder => (int)BodyTrackingBones.RightShoulder,
                (int)HumanBodyBones.RightUpperArm => (int)BodyTrackingBones.RightUpperArm,
                (int)HumanBodyBones.RightLowerArm => (int)BodyTrackingBones.RightLowerArm,
                (int)HumanBodyBones.RightHand     => (int)BodyTrackingBones.RightHand,
                (int)HumanBodyBones.LeftUpperLeg  => (int)BodyTrackingBones.LeftUpperLeg,
                (int)HumanBodyBones.LeftLowerLeg  => (int)BodyTrackingBones.LeftLowerLeg,
                (int)HumanBodyBones.LeftFoot      => (int)BodyTrackingBones.LeftFoot,
                (int)HumanBodyBones.LeftToes      => (int)BodyTrackingBones.LeftToes,
                (int)HumanBodyBones.RightUpperLeg => (int)BodyTrackingBones.RightUpperLeg,
                (int)HumanBodyBones.RightLowerLeg => (int)BodyTrackingBones.RightLowerLeg,
                (int)HumanBodyBones.RightFoot     => (int)BodyTrackingBones.RightFoot,
                (int)HumanBodyBones.RightToes     => (int)BodyTrackingBones.RightToes,
                _ => -1,
            };
            return bodyTrackingBoneId;
        }

        public static int GetFingerTrackingBoneId(int humanBodyBoneId)
        {
            var fingerTrackingBoneId = humanBodyBoneId switch
            {
                // Left hand
                (int)HumanBodyBones.LeftHand                => (int)FingerTrackingBones.LeftHand,
                (int)HumanBodyBones.LeftThumbProximal       => (int)FingerTrackingBones.LeftThumbProximal,
                (int)HumanBodyBones.LeftThumbIntermediate   => (int)FingerTrackingBones.LeftThumbIntermediate,
                (int)HumanBodyBones.LeftThumbDistal         => (int)FingerTrackingBones.LeftThumbDistal,
                (int)HumanBodyBones.LeftIndexProximal       => (int)FingerTrackingBones.LeftIndexProximal,
                (int)HumanBodyBones.LeftIndexIntermediate   => (int)FingerTrackingBones.LeftIndexIntermediate,
                (int)HumanBodyBones.LeftIndexDistal         => (int)FingerTrackingBones.LeftIndexDistal,
                (int)HumanBodyBones.LeftMiddleProximal      => (int)FingerTrackingBones.LeftMiddleProximal,
                (int)HumanBodyBones.LeftMiddleIntermediate  => (int)FingerTrackingBones.LeftMiddleIntermediate,
                (int)HumanBodyBones.LeftMiddleDistal        => (int)FingerTrackingBones.LeftMiddleDistal,
                (int)HumanBodyBones.LeftRingProximal        => (int)FingerTrackingBones.LeftRingProximal,
                (int)HumanBodyBones.LeftRingIntermediate    => (int)FingerTrackingBones.LeftRingIntermediate,
                (int)HumanBodyBones.LeftRingDistal          => (int)FingerTrackingBones.LeftRingDistal,
                (int)HumanBodyBones.LeftLittleProximal      => (int)FingerTrackingBones.LeftLittleProximal,
                (int)HumanBodyBones.LeftLittleIntermediate  => (int)FingerTrackingBones.LeftLittleIntermediate,
                (int)HumanBodyBones.LeftLittleDistal        => (int)FingerTrackingBones.LeftLittleDistal,
                // Right hand
                (int)HumanBodyBones.RightHand               => (int)FingerTrackingBones.RightHand,
                (int)HumanBodyBones.RightThumbProximal      => (int)FingerTrackingBones.RightThumbProximal,
                (int)HumanBodyBones.RightThumbIntermediate  => (int)FingerTrackingBones.RightThumbIntermediate,
                (int)HumanBodyBones.RightThumbDistal        => (int)FingerTrackingBones.RightThumbDistal,
                (int)HumanBodyBones.RightIndexProximal      => (int)FingerTrackingBones.RightIndexProximal,
                (int)HumanBodyBones.RightIndexIntermediate  => (int)FingerTrackingBones.RightIndexIntermediate,
                (int)HumanBodyBones.RightIndexDistal        => (int)FingerTrackingBones.RightIndexDistal,
                (int)HumanBodyBones.RightMiddleProximal     => (int)FingerTrackingBones.RightMiddleProximal,
                (int)HumanBodyBones.RightMiddleIntermediate => (int)FingerTrackingBones.RightMiddleIntermediate,
                (int)HumanBodyBones.RightMiddleDistal       => (int)FingerTrackingBones.RightMiddleDistal,
                (int)HumanBodyBones.RightRingProximal       => (int)FingerTrackingBones.RightRingProximal,
                (int)HumanBodyBones.RightRingIntermediate   => (int)FingerTrackingBones.RightRingIntermediate,
                (int)HumanBodyBones.RightRingDistal         => (int)FingerTrackingBones.RightRingDistal,
                (int)HumanBodyBones.RightLittleProximal     => (int)FingerTrackingBones.RightLittleProximal,
                (int)HumanBodyBones.RightLittleIntermediate => (int)FingerTrackingBones.RightLittleIntermediate,
                (int)HumanBodyBones.RightLittleDistal       => (int)FingerTrackingBones.RightLittleDistal,
                // Others
                _ => -1,
            };
            return fingerTrackingBoneId;
        }
    }
}
