using MocapSignalTransmission.MotionData;
using MocapSignalTransmission.MotionDataSource;
using UnityEngine;
using UnityEngine.Assertions;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class MocopiDataBuffer : IBodyTrackingDataSource
    {
        private readonly BodyTrackingFrame[] _bodyTrackingFrameBuffer;
        private readonly int _bufferSize;
        private readonly long _bufferMask;

        private long _bufferHead = 0;
        private long _bufferTail = 0;

        public int Id { get; }

        public MocopiDataBuffer(int id, int bufferSize = 2)
        {
            Assert.IsTrue(Utils.IsPowerOfTwo(bufferSize), "The buffer size must be a power of two.");
            _bufferSize = bufferSize;
            _bufferMask = bufferSize - 1;

            Id = id;

            _bodyTrackingFrameBuffer = new BodyTrackingFrame[bufferSize];

            for (var i = 0; i < bufferSize; i++)
            {
                _bodyTrackingFrameBuffer[i] = new BodyTrackingFrame();
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
        /// Initialize avatar bone data
        /// </summary>
        /// <param name="boneIds">mocopi Avatar bone id list</param>
        /// <param name="parentJointIds">List of IDs of parent bones for each bone</param>
        /// <param name="rotationsX">Rotation angle of each bone in initial posture</param>
        /// <param name="rotationsY">Rotation angle of each bone in initial posture</param>
        /// <param name="rotationsZ">Rotation angle of each bone in initial posture</param>
        /// <param name="rotationsW">Rotation angle of each bone in initial posture</param>
        /// <param name="positionsX">Position of each bone in initial pose</param>
        /// <param name="positionsY">Position of each bone in initial pose</param>
        /// <param name="positionsZ">Position of each bone in initial pose</param>
        public void InitializeSkeleton(
            int[] boneIds, int[] parentBoneIds,
            float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
            float[] positionsX, float[] positionsY, float[] positionsZ)
        {
            var enqueueIndex = _bufferTail & _bufferMask;

            for (var i = 0; i < boneIds.Length; i++)
            {
                var actorBoneId = GetBodyTrackingBoneId((MocopiBones)boneIds[i]);
                if (actorBoneId < 0) continue;

                // Convert to Unity coordinates
                var position = new Vector3(-positionsX[i], positionsY[i], positionsZ[i]);
                var rotation = new Quaternion(-rotationsX[i], rotationsY[i], rotationsZ[i], -rotationsW[i]);

                // _bodyTrackingFrameBuffer[enqueueIndex].BonePositions[actorBoneId] = position;
                _bodyTrackingFrameBuffer[enqueueIndex].BoneRotations[actorBoneId] = rotation;

                if (actorBoneId == (int)HumanBodyBones.Hips)
                {
                    _bodyTrackingFrameBuffer[enqueueIndex].RootPosition = position;
                    _bodyTrackingFrameBuffer[enqueueIndex].RootRotation = rotation;
                }
            }

            // Update the enqueue position to insert the next data.
            _bufferTail++;

            // When the buffer overflows, the head data (oldest data) is deleted.
            var bufferFreeCount = _bufferSize - (int)(_bufferTail - _bufferHead);
            if (bufferFreeCount <= 0)
            {
                _bufferHead++;
            }
        }

        /// <summary>
        /// Update avatar bone data
        /// </summary>
        /// <param name="frameId">Frame Id</param>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="unixTime">Unix time when sensor sent data</param>
        /// <param name="boneIds">mocopi Avatar bone id list</param>
        /// <param name="rotationsX">Rotation angle of each bone</param>
        /// <param name="rotationsY">Rotation angle of each bone</param>
        /// <param name="rotationsZ">Rotation angle of each bone</param>
        /// <param name="rotationsW">Rotation angle of each bone</param>
        /// <param name="positionsX">Position of each bone</param>
        /// <param name="positionsY">Position of each bone</param>
        /// <param name="positionsZ">Position of each bone</param>
        public void UpdateSkeleton(
            int frameId, float timestamp, double unixTime,
            int[] mocopiBoneIds,
            float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
            float[] positionsX, float[] positionsY, float[] positionsZ)
        {
            var enqueueIndex = _bufferTail & _bufferMask;

            for (var i = 0; i < mocopiBoneIds.Length; i++)
            {
                var actorBoneId = GetBodyTrackingBoneId((MocopiBones)mocopiBoneIds[i]);
                if (actorBoneId < 0) continue;

                // Convert to Unity coordinates
                var position = new Vector3(-positionsX[i], positionsY[i], positionsZ[i]);
                var rotation = new Quaternion(-rotationsX[i], rotationsY[i], rotationsZ[i], -rotationsW[i]);

                // _bodyTrackingFrameBuffer[enqueueIndex].BonePositions[actorBoneId] = position;
                _bodyTrackingFrameBuffer[enqueueIndex].BoneRotations[actorBoneId] = rotation;

                if (actorBoneId == (int)HumanBodyBones.Hips)
                {
                    _bodyTrackingFrameBuffer[enqueueIndex].RootPosition = position;
                    _bodyTrackingFrameBuffer[enqueueIndex].RootRotation = rotation;
                }
            }

            // Update the enqueue position to insert the next data.
            _bufferTail++;

            // When the buffer overflows, the head data (oldest data) is deleted.
            var bufferFreeCount = _bufferSize - (int)(_bufferTail - _bufferHead);
            if (bufferFreeCount <= 0)
            {
                _bufferHead++;
            }
        }

        //
        // References:
        // - https://www.sony.net/Products/mocopi-dev/jp/documents/Home/TechSpec.html
        // - https://www.sony.net/Products/mocopi-dev/en/documents/Home/TechSpec.html
        //
        public static int GetBodyTrackingBoneId(MocopiBones mocopiBone)
        {
            var motionActorBoneId = mocopiBone switch
            {
                MocopiBones.root          => (int)BodyTrackingBones.Hips,
                MocopiBones.torso_3       => (int)BodyTrackingBones.Spine,
                MocopiBones.torso_5       => (int)BodyTrackingBones.Chest,
                MocopiBones.torso_6       => (int)BodyTrackingBones.UpperChest,
                MocopiBones.neck_1        => (int)BodyTrackingBones.Neck,
                MocopiBones.head          => (int)BodyTrackingBones.Head,
                MocopiBones.l_shoulder    => (int)BodyTrackingBones.LeftShoulder,
                MocopiBones.l_up_arm      => (int)BodyTrackingBones.LeftUpperArm,
                MocopiBones.l_low_arm     => (int)BodyTrackingBones.LeftLowerArm,
                MocopiBones.l_hand        => (int)BodyTrackingBones.LeftHand,
                MocopiBones.r_shoulder    => (int)BodyTrackingBones.RightShoulder,
                MocopiBones.r_up_arm      => (int)BodyTrackingBones.RightUpperArm,
                MocopiBones.r_low_arm     => (int)BodyTrackingBones.RightLowerArm,
                MocopiBones.r_hand        => (int)BodyTrackingBones.RightHand,
                MocopiBones.l_up_leg      => (int)BodyTrackingBones.LeftUpperLeg,
                MocopiBones.l_low_leg     => (int)BodyTrackingBones.LeftLowerLeg,
                MocopiBones.l_foot        => (int)BodyTrackingBones.LeftFoot,
                MocopiBones.l_toes        => (int)BodyTrackingBones.LeftToes,
                MocopiBones.r_up_leg      => (int)BodyTrackingBones.RightUpperLeg,
                MocopiBones.r_low_leg     => (int)BodyTrackingBones.RightLowerLeg,
                MocopiBones.r_foot        => (int)BodyTrackingBones.RightFoot,
                MocopiBones.r_toes        => (int)BodyTrackingBones.RightToes,
                _ => -1,
            };
            return motionActorBoneId;
        }

        public enum MocopiBones
        {
            root =       0,
            torso_1 =    1,
            torso_2 =    2,
            torso_3 =    3,
            torso_4 =    4,
            torso_5 =    5,
            torso_6 =    6,
            torso_7 =    7,
            neck_1 =     8,
            neck_2 =     9,
            head =       10,
            l_shoulder = 11,
            l_up_arm =   12,
            l_low_arm =  13,
            l_hand =     14,
            r_shoulder = 15,
            r_up_arm =   16,
            r_low_arm =  17,
            r_hand =     18,
            l_up_leg =   19,
            l_low_leg =  20,
            l_foot =     21,
            l_toes =     22,
            r_up_leg =   23,
            r_low_leg =  24,
            r_foot =     25,
            r_toes =     26,
        }
    }
}
