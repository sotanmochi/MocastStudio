using System;
using System.Buffers;
using MessagePack;
using MocapSignalTransmission.Infrastructure.Constants;
using MocapSignalTransmission.MotionData;
using MocapSignalTransmission.MotionDataSource;
using UnityEngine;
using UnityEngine.Assertions;

namespace MocapSignalTransmission.Infrastructure.MotionDataSource
{
    public sealed class MocastStudioDataBuffer : IHumanPoseTrackingDataSource
    {
        public static readonly int MotionCaptureMessageId = (int)SignalType.MotionCaptureData;
        public static readonly int MuscleCount = HumanTrait.MuscleCount;

        readonly ActorHumanPose[] _humanPoseDataBuffer;
        readonly int _bufferSize;
        readonly long _bufferMask;

        long _bufferHead = 0;
        long _bufferTail = 0;

        public int Id { get; }
        public uint StreamingDataId { get; }

        public MocastStudioDataBuffer(int id, uint streamingDataId, int bufferSize = 2)
        {
            Assert.IsTrue(Utils.IsPowerOfTwo(bufferSize), "The buffer size must be a power of two.");
            _bufferSize = bufferSize;
            _bufferMask = bufferSize - 1;

            Id = id;
            StreamingDataId = streamingDataId;

            _humanPoseDataBuffer = new ActorHumanPose[bufferSize];

            for (var i = 0; i < bufferSize; i++)
            {
                _humanPoseDataBuffer[i].Muscles = new float[MuscleCount];
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

            humanPoseFrame.bodyPosition = _humanPoseDataBuffer[bufferIndex].BodyPosition;
            humanPoseFrame.bodyRotation = _humanPoseDataBuffer[bufferIndex].BodyRotation;
            for (var i = 0; i < humanPoseFrame.muscles.Length; i++)
            {
                humanPoseFrame.muscles[i] = _humanPoseDataBuffer[bufferIndex].Muscles[i];
            }

            return true;
        }

        public void Enqueue(int messageId, ReadOnlySequence<byte> serializedMessage, uint senderClientId)
        {
            if (messageId != MotionCaptureMessageId) return;

            var actorHumanPose = MessagePackSerializer.Deserialize<ActorHumanPose>(serializedMessage);
            if (actorHumanPose.ActorId != StreamingDataId) return;

            var enqueueIndex = _bufferTail & _bufferMask;
            _humanPoseDataBuffer[enqueueIndex] = actorHumanPose;

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
