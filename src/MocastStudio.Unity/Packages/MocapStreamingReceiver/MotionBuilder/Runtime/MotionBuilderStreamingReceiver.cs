using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CrescentStreaming;
using Debug = UnityEngine.Debug;
using UnityEngine;

namespace MocapStreamingReceiver.MotionBuilder
{
    public sealed class MotionBuilderStreamingReceiver : IDisposable
    {
        public sealed class SkeletonData
        {
            public int ObjectId = -1;
            public string Name = "Skeleton";

            public Vector3 Scale = Vector3.one;

            /// <summary>
            /// World space position of the root bone.
            /// </summary>
            public Vector3 RootBonePosition = Vector3.zero;

            /// <summary>
            /// World space rotation of the root bone.
            /// </summary>
            public Quaternion RootBoneRotation = Quaternion.identity;

            /// <summary>
            /// Local space rotations of humanoid bones (including the root bone).
            /// </summary>
            public Quaternion[] BoneRotations = new Quaternion[HumanoidBoneCount];
        }

        private static readonly int HumanoidBoneCount = (int)HumanBodyBones.LastBone;
        private static readonly double TimestampsToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
        private static readonly Quaternion ZeroQuaternion = new Quaternion(0F, 0F, 0F, 0F);

        private readonly Thread _receiverLoopThread;
        private readonly CancellationTokenSource _receiverLoopCts;
        private readonly int _targetFrameTimeMilliseconds;

        private readonly List<SkeletonData> _skeletonDataList = new();
        private readonly Quaternion[] _boneRotationsBuffer = new Quaternion[HumanoidBoneCount];

        private TaskCompletionSource<bool> _connectionTcs;
        private CCrescentStreamClient _client;
        
        public event Action<SkeletonData> OnDataReceived;
        
        public bool IsConnected => _client.IsConnected;

        public MotionBuilderStreamingReceiver(int targetFrameRate, bool isBackground)
        {
            _targetFrameTimeMilliseconds = (int)(1000 / (double)targetFrameRate);
            
            _receiverLoopCts = new CancellationTokenSource();
            _receiverLoopThread = new Thread(ReceiverLoop)
            {
                Name = $"{typeof(MotionBuilderStreamingReceiver).Name}",
                IsBackground = isBackground,
            };

            _client = new CCrescentStreamClient();
            _client.OnConnected += OnConnected;
            _client.OnDisconnected += OnDisconnected;
            _client.OnDataUpdated += OnDataUpdated;
            _client.OnReceiveThreadStart += OnReceiveThreadStart;
            _client.OnConnectSuspended += OnConnectSuspended;

            _receiverLoopThread.Start();
        }

        public void Dispose()
        {
            _receiverLoopCts.Cancel();
            _receiverLoopCts.Dispose();
            _client.Disconnect();
            _client = null;
        }

        public async Task<bool> ConnectAsync(string serverAddress = "127.0.0.1", int serverPort = 22000, CancellationToken cancellationToken = default)
        {
            _connectionTcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => _connectionTcs.TrySetCanceled());
            _client.ConnectAsync(serverAddress, serverPort);
            return await _connectionTcs.Task;
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        private void ReceiverLoop()
        {
            while (!_receiverLoopCts.IsCancellationRequested)
            {
                var begin = Stopwatch.GetTimestamp();

                try
                {
                    HandleIncomingData();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                var end = Stopwatch.GetTimestamp();
                var elapsedTicks = (end - begin) * TimestampsToTicks;
                var elapsedMilliseconds = (long)elapsedTicks / TimeSpan.TicksPerMillisecond;

                var waitForNextFrameMilliseconds = (int)(_targetFrameTimeMilliseconds - elapsedMilliseconds);
                if (waitForNextFrameMilliseconds > 0)
                {
                    Thread.Sleep(waitForNextFrameMilliseconds);
                }
            }
        }

        private void HandleIncomingData()
        {
            float[] streamDataValues = null;
            
            if (!_client.IsConnected) return;

            var objectDataCount = _client.GetStreamDataCount(); // CharacterCount + ObjectCount + BlendShapeObjectCount
            var characterCount = _client.GetCharacterCount();
            if (objectDataCount <= 0 || characterCount <= 0) return;
            
            if (characterCount > _skeletonDataList.Count)
            {
                var countDiff = characterCount - _skeletonDataList.Count;
                for (var i = 0; i < countDiff; i++)
                {
                    _skeletonDataList.Add(new SkeletonData());
                }
            }

            var characterIndex = 0;
            for (var objectIndex = 0; objectIndex < objectDataCount; objectIndex++)
            {
                var dataType = _client.GetStreamDataType(objectIndex);

                if (dataType == StreamDataType.HumanIK)
                {
                    _skeletonDataList[characterIndex].ObjectId = objectIndex;
                    _skeletonDataList[characterIndex].Name = _client.GetStreamDataName(objectIndex);

                    if (_client.GetStreamData(objectIndex, ref streamDataValues))
                    {
                        var offset = 0;
                        for (var boneId = 0; boneId < HumanoidBoneCount; boneId++)
                        {
                            if (boneId == 0) // NOTE: (int)HumanBodyBones.Hips is 0
                            {
                                var posX = streamDataValues[offset + 0];
                                var posY = streamDataValues[offset + 1];
                                var posZ = streamDataValues[offset + 2];
                                var rotX = streamDataValues[offset + 3];
                                var rotY = streamDataValues[offset + 4];
                                var rotZ = streamDataValues[offset + 5];
                                var rotW = streamDataValues[offset + 6];
                                var scaleX = streamDataValues[offset + 7];
                                var scaleY = streamDataValues[offset + 8];
                                var scaleZ = streamDataValues[offset + 9];

                                offset += 10;

                                var positionHasNan = double.IsNaN(posX) || double.IsNaN(posY) || double.IsNaN(posZ);
                                var rotationHasNan = double.IsNaN(rotX) || double.IsNaN(rotY) || double.IsNaN(rotZ) || double.IsNaN(rotW);
                                var scaleHasNan = double.IsNaN(scaleX) || double.IsNaN(scaleY) || double.IsNaN(scaleZ);

                                var isInvalid = positionHasNan || rotationHasNan || scaleHasNan;
                                if (isInvalid) continue;

                                // Convert to Unity coordinates
                                var position = new Vector3(-0.01f * posX, 0.01f * posY, 0.01f * posZ);
                                var rotation = new Quaternion(-rotX, rotY, rotZ, -rotW);
                                var scale = new Vector3(scaleX, scaleY, scaleZ);

                                _boneRotationsBuffer[boneId] = rotation; // World rotation
                                _skeletonDataList[characterIndex].RootBonePosition = position;
                                _skeletonDataList[characterIndex].RootBoneRotation = rotation;
                                _skeletonDataList[characterIndex].Scale = scale;
                            }
                            else
                            {
                                var rotX = streamDataValues[offset + 0];
                                var rotY = streamDataValues[offset + 1];
                                var rotZ = streamDataValues[offset + 2];
                                var rotW = streamDataValues[offset + 3];

                                offset += 4;

                                var rotationHasNan = double.IsNaN(rotX) || double.IsNaN(rotY) || double.IsNaN(rotZ) || double.IsNaN(rotW);
                                if (rotationHasNan) continue;

                                var rotation = new Quaternion(-rotX, rotY, rotZ, -rotW); // Convert to Unity coordinates
                                _boneRotationsBuffer[boneId] = rotation; // World rotation
                            }
                        }

                        // Calculate local rotations
                        for (var boneId = 0; boneId < HumanoidBoneCount; boneId++)
                        {
                            var parentBoneId = GetParentBoneId(boneId);

                            // Get parent bone id recursively, if the parent bone has not yet been received.
                            while (parentBoneId >= 0 && parentBoneId != boneId && _boneRotationsBuffer[parentBoneId] == ZeroQuaternion)
                            {
                                parentBoneId = GetParentBoneId(parentBoneId);
                            }

                            if (parentBoneId >= 0 && parentBoneId == boneId)
                            {
                                _skeletonDataList[characterIndex].BoneRotations[boneId] = _boneRotationsBuffer[boneId];
                            }
                            else if (parentBoneId >= 0 && parentBoneId != boneId)
                            {
                                var boneRotation = _boneRotationsBuffer[boneId];
                                var parentBoneRotation = _boneRotationsBuffer[parentBoneId];

                                var localRotation = Quaternion.Inverse(parentBoneRotation) * boneRotation;
                                _skeletonDataList[characterIndex].BoneRotations[boneId] = localRotation;
                            }
                        }

                        ClearBuffer();
                    }

                    OnDataReceived?.Invoke(_skeletonDataList[characterIndex]);
                    characterIndex++;
                }
            }
        }

        private void ClearBuffer()
        {
            for (var i = 0; i < _boneRotationsBuffer.Length; i++)
            {
                _boneRotationsBuffer[i] = ZeroQuaternion;
            }
        }

#region Callbacks

        private void OnConnected(object sender, EventArgs e, bool connected)
        {
            Debug.Log($"<color=cyan>[{nameof(MotionBuilderStreamingReceiver)}] OnConnected callback result: {connected}</color>");
            _connectionTcs.SetResult(connected);
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
        }

        private void OnDataUpdated(object sender, EventArgs e)
        {
        }

        private void OnReceiveThreadStart(object sender, EventArgs e)
        {
        }

        private void OnConnectSuspended(object sender, EventArgs e)
        {
        }

#endregion

        private static int GetParentBoneId(int boneId)
        {
            var parentBoneId = boneId switch
            {
                // Body
                (int)HumanBodyBones.Hips                    => (int)HumanBodyBones.Hips,
                (int)HumanBodyBones.Spine                   => (int)HumanBodyBones.Hips,
                (int)HumanBodyBones.Chest                   => (int)HumanBodyBones.Spine,
                (int)HumanBodyBones.UpperChest              => (int)HumanBodyBones.Chest,
                (int)HumanBodyBones.Neck                    => (int)HumanBodyBones.UpperChest,
                (int)HumanBodyBones.Head                    => (int)HumanBodyBones.Neck,
                (int)HumanBodyBones.LeftShoulder            => (int)HumanBodyBones.UpperChest,
                (int)HumanBodyBones.LeftUpperArm            => (int)HumanBodyBones.LeftShoulder,
                (int)HumanBodyBones.LeftLowerArm            => (int)HumanBodyBones.LeftUpperArm,
                (int)HumanBodyBones.LeftHand                => (int)HumanBodyBones.LeftLowerArm,
                (int)HumanBodyBones.RightShoulder           => (int)HumanBodyBones.UpperChest,
                (int)HumanBodyBones.RightUpperArm           => (int)HumanBodyBones.RightShoulder,
                (int)HumanBodyBones.RightLowerArm           => (int)HumanBodyBones.RightUpperArm,
                (int)HumanBodyBones.RightHand               => (int)HumanBodyBones.RightLowerArm,
                (int)HumanBodyBones.LeftUpperLeg            => (int)HumanBodyBones.Hips,
                (int)HumanBodyBones.LeftLowerLeg            => (int)HumanBodyBones.LeftUpperLeg,
                (int)HumanBodyBones.LeftFoot                => (int)HumanBodyBones.LeftLowerLeg,
                (int)HumanBodyBones.LeftToes                => (int)HumanBodyBones.LeftFoot,
                (int)HumanBodyBones.RightUpperLeg           => (int)HumanBodyBones.Hips,
                (int)HumanBodyBones.RightLowerLeg           => (int)HumanBodyBones.RightUpperLeg,
                (int)HumanBodyBones.RightFoot               => (int)HumanBodyBones.RightLowerLeg,
                (int)HumanBodyBones.RightToes               => (int)HumanBodyBones.RightFoot,
                // Left hand
                (int)HumanBodyBones.LeftThumbProximal       => (int)HumanBodyBones.LeftHand,
                (int)HumanBodyBones.LeftThumbIntermediate   => (int)HumanBodyBones.LeftThumbProximal,
                (int)HumanBodyBones.LeftThumbDistal         => (int)HumanBodyBones.LeftThumbIntermediate,
                (int)HumanBodyBones.LeftIndexProximal       => (int)HumanBodyBones.LeftHand,
                (int)HumanBodyBones.LeftIndexIntermediate   => (int)HumanBodyBones.LeftIndexProximal,
                (int)HumanBodyBones.LeftIndexDistal         => (int)HumanBodyBones.LeftIndexIntermediate,
                (int)HumanBodyBones.LeftMiddleProximal      => (int)HumanBodyBones.LeftHand,
                (int)HumanBodyBones.LeftMiddleIntermediate  => (int)HumanBodyBones.LeftMiddleProximal,
                (int)HumanBodyBones.LeftMiddleDistal        => (int)HumanBodyBones.LeftMiddleIntermediate,
                (int)HumanBodyBones.LeftRingProximal        => (int)HumanBodyBones.LeftHand,
                (int)HumanBodyBones.LeftRingIntermediate    => (int)HumanBodyBones.LeftRingProximal,
                (int)HumanBodyBones.LeftRingDistal          => (int)HumanBodyBones.LeftRingIntermediate,
                (int)HumanBodyBones.LeftLittleProximal      => (int)HumanBodyBones.LeftHand,
                (int)HumanBodyBones.LeftLittleIntermediate  => (int)HumanBodyBones.LeftLittleProximal,
                (int)HumanBodyBones.LeftLittleDistal        => (int)HumanBodyBones.LeftLittleIntermediate,
                // Right hand
                (int)HumanBodyBones.RightThumbProximal      => (int)HumanBodyBones.RightHand,
                (int)HumanBodyBones.RightThumbIntermediate  => (int)HumanBodyBones.RightThumbProximal,
                (int)HumanBodyBones.RightThumbDistal        => (int)HumanBodyBones.RightThumbIntermediate,
                (int)HumanBodyBones.RightIndexProximal      => (int)HumanBodyBones.RightHand,
                (int)HumanBodyBones.RightIndexIntermediate  => (int)HumanBodyBones.RightIndexProximal,
                (int)HumanBodyBones.RightIndexDistal        => (int)HumanBodyBones.RightIndexIntermediate,
                (int)HumanBodyBones.RightMiddleProximal     => (int)HumanBodyBones.RightHand,
                (int)HumanBodyBones.RightMiddleIntermediate => (int)HumanBodyBones.RightMiddleProximal,
                (int)HumanBodyBones.RightMiddleDistal       => (int)HumanBodyBones.RightMiddleIntermediate,
                (int)HumanBodyBones.RightRingProximal       => (int)HumanBodyBones.RightHand,
                (int)HumanBodyBones.RightRingIntermediate   => (int)HumanBodyBones.RightRingProximal,
                (int)HumanBodyBones.RightRingDistal         => (int)HumanBodyBones.RightRingIntermediate,
                (int)HumanBodyBones.RightLittleProximal     => (int)HumanBodyBones.RightHand,
                (int)HumanBodyBones.RightLittleIntermediate => (int)HumanBodyBones.RightLittleProximal,
                (int)HumanBodyBones.RightLittleDistal       => (int)HumanBodyBones.RightLittleIntermediate,
                _ => -1,
            };
            return parentBoneId;
        }
    }
}
