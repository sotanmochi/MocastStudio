using MocapSignalTransmission.MotionDataSource;
using UnityEngine;
using UnityEngine.Assertions;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class MocastStudioDataBuffer : IHumanPoseTrackingDataSource
    {
        private readonly HumanPose[] _humanPoseFrameBuffer;
        private readonly int _bufferSize;
        private readonly long _bufferMask;

        private long _bufferHead = 0;
        private long _bufferTail = 0;

        public int Id { get; }

        public MocastStudioDataBuffer(int id, int bufferSize = 2)
        {
            Assert.IsTrue(Utils.IsPowerOfTwo(bufferSize), "The buffer size must be a power of two.");
            _bufferSize = bufferSize;
            _bufferMask = bufferSize - 1;

            Id = id;

            _humanPoseFrameBuffer = new HumanPose[bufferSize];

            for (var i = 0; i < bufferSize; i++)
            {
                _humanPoseFrameBuffer[i].muscles = new float[HumanTrait.MuscleCount];
            }
        }

        /// <summary>
        /// Try to get a reference of the first data in the buffer.
        /// The data is not deleted from the buffer in this method.
        /// When the buffer overflows during enqueue, the first data (oldest data) is automatically deleted.
        /// </summary>
        /// <param name="humanPoseFrame"></param>
        /// <returns></returns>
        public bool TryPeek(ref HumanPose humanPoseFrame)
        {
            var bufferIndex = _bufferHead & _bufferMask;
            humanPoseFrame = _humanPoseFrameBuffer[bufferIndex];
            return true;
        }

        public void Enqueue(HumanPose humanPose)
        {
            var enqueueIndex = _bufferTail & _bufferMask;

            _humanPoseFrameBuffer[enqueueIndex] = humanPose;

            // _humanPoseFrameBuffer[enqueueIndex].bodyPosition = bodyPosition;
            // _humanPoseFrameBuffer[enqueueIndex].bodyRotation = bodyRotation;
            // for (var i = 0; i < muscles.Length; i++)
            // {
            //     _humanPoseFrameBuffer[enqueueIndex].muscles[i] = muscles[i];
            // }

            // Update the enqueue position to insert the next data.
            _bufferTail++;

            // When the buffer overflows, the head data (oldest data) is deleted.
            var bufferFreeCount = _bufferSize - (int)(_bufferTail - _bufferHead);
            if (bufferFreeCount <= 0)
            {
                _bufferHead++;
            }
        }
    }
}
