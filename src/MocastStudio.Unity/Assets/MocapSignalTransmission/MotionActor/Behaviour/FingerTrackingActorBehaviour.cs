using MocapSignalTransmission.MotionData;
using UnityEngine;

namespace MocapSignalTransmission.MotionActor
{
    public sealed class FingerTrackingActorBehaviour : MonoBehaviour
    {
        [SerializeField] bool _autoInitialize;
        [SerializeField] Animator _animator;

        private bool _initialized;
        private TransformReference[] _bones = new TransformReference[(int)FingerTrackingBones.Count];

        public bool Initialized => _initialized;
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
                for (var i = 0; i < FingerTrackingFrame.BoneCount; i++)
                {                
                    var humanBodyBone = FingerTrackingHelper.GetHumanBodyBone(i);
                    var boneTransform = _animator.GetBoneTransform(humanBodyBone);
                    _bones[i] = new TransformReference(humanBodyBone.ToString(), boneTransform);
                }
                _initialized = true;
            }
        }

        public void UpdatePose(FingerTrackingFrame frame)
        {
            if (!_initialized) return;

            for (var boneId = 0; boneId < FingerTrackingFrame.BoneCount; boneId++)
            {
                if (boneId != (int)FingerTrackingBones.LeftHand && boneId != (int)FingerTrackingBones.RightHand)
                {
                    _bones[boneId].Transform.localRotation = frame.BoneRotations[boneId];
                }
            }
        }
    }
}
