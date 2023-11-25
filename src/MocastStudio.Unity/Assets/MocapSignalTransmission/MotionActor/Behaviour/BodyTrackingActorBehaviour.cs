using MocapSignalTransmission.MotionData;
using UnityEngine;

namespace MocapSignalTransmission.MotionActor
{
    public sealed class BodyTrackingActorBehaviour : MonoBehaviour
    {
        [SerializeField] bool _autoInitialize;
        [SerializeField] Animator _animator;

        private bool _initialized;
        private Transform[] _bones = new Transform[(int)BodyTrackingBones.Count];

        public bool Initialized => _initialized;

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

        public void UpdatePose(BodyTrackingFrame frame)
        {
            if (!_initialized) return;

            for (var boneId = 0; boneId < BodyTrackingFrame.BoneCount; boneId++)
            {
                _bones[boneId].localRotation = frame.BoneRotations[boneId];
            }

            _bones[(int)BodyTrackingBones.Hips].localPosition = frame.RootPosition;
            _bones[(int)BodyTrackingBones.Hips].localRotation = frame.RootRotation;
        }
    }
}
