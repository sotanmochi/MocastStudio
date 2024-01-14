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
        private TransformReference[] _bones = new TransformReference[(int)BodyTrackingBones.Count];
        private Vector3 _rootBonePositionOffset = Vector3.zero;
        private Quaternion _rootBoneRotationOffset = Quaternion.identity;

        public bool Initialized => _initialized;
        public bool RootBoneOffsetEnabled { get; set; }
        public TransformReference[] Bones => _bones;

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
                    var boneTransform = _animator.GetBoneTransform(humanBodyBone);
                    _bones[i] = new TransformReference(humanBodyBone.ToString(), boneTransform);
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
                _bones[boneId].Transform.localRotation = frame.BoneRotations[boneId];
            }
 
            if (RootBoneOffsetEnabled)
            {
                _bones[RootBoneId].Transform.localPosition = _rootBonePositionOffset + frame.RootPosition;
                _bones[RootBoneId].Transform.localRotation = _rootBoneRotationOffset * frame.RootRotation;
            }
            else
            {
                _bones[RootBoneId].Transform.localPosition = frame.RootPosition;
                _bones[RootBoneId].Transform.localRotation = frame.RootRotation;
            }
        }
    }
}
