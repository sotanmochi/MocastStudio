using MocapSignalTransmission.MotionData;
using UnityEngine;

namespace MocapSignalTransmission.MotionActor
{
    public sealed class BodyTrackingActorBehaviour : MonoBehaviour
    {
        private static readonly int RootBoneId = (int)BodyTrackingBones.Hips;

        [SerializeField] bool _autoInitialize;
        [SerializeField] Animator _animator;

        private bool _initialized;
        private Transform[] _bones = new Transform[(int)BodyTrackingBones.Count];
        private Vector3 _rootBonePositionOffset = Vector3.zero;
        private Quaternion _rootBoneRotationOffset = Quaternion.identity;

        public bool Initialized => _initialized;
        public bool RootBoneOffsetEnabled { get; set; }

        void Awake()
        {
            if (_autoInitialize)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (_initialized) return;

            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }

            if (_animator != null)
            {
                for (var i = 0; i < BodyTrackingFrame.BoneCount; i++)
                {                
                    var humanBodyBone = BodyTrackingHelper.GetHumanBodyBone(i);
                    _bones[i] = _animator.GetBoneTransform(humanBodyBone);
                }
                _initialized = true;
            }
        }

        public void UpdateRootBoneOffset(Vector3 position, Quaternion rotation)
        {
            _rootBonePositionOffset = position;
            _rootBoneRotationOffset = rotation;
        }

        public void UpdatePose(BodyTrackingFrame frame)
        {
            if (!_initialized) return;

            for (var boneId = 0; boneId < BodyTrackingFrame.BoneCount; boneId++)
            {
                _bones[boneId].localRotation = frame.BoneRotations[boneId];
            }
 
            if (RootBoneOffsetEnabled)
            {
                _bones[RootBoneId].localPosition = _rootBonePositionOffset + frame.RootPosition;
                _bones[RootBoneId].localRotation = _rootBoneRotationOffset * frame.RootRotation;
            }
            else
            {
                _bones[RootBoneId].localPosition = frame.RootPosition;
                _bones[RootBoneId].localRotation = frame.RootRotation;
            }
        }
    }
}
