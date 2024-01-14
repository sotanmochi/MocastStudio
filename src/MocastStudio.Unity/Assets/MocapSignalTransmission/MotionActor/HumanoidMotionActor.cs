using UnityEngine;

namespace MocapSignalTransmission.MotionActor
{
    public sealed class HumanoidMotionActor : IMotionActor
    {
        private readonly int _actorId;
        private readonly string _name;
        private readonly HumanPoseHandler _humanPoseHandler;
        private readonly BodyTrackingActorBehaviour _bodyTrackingActorBehaviour;
        private readonly FingerTrackingActorBehaviour _fingerTrackingActorBehaviour;

        private HumanPose _humanPose;

        public int ActorId => _actorId;
        public string Name => _name;

        public HumanPose HumanPose => _humanPose;
        public Transform ActorSpaceReferenceTransform => _bodyTrackingActorBehaviour.transform; // NOTE:

        public BodyTrackingActorBehaviour BodyTrackingActorBehaviour => _bodyTrackingActorBehaviour;
        public FingerTrackingActorBehaviour FingerTrackingActorBehaviour => _fingerTrackingActorBehaviour;

        public TransformReference[] BodyBones => _bodyTrackingActorBehaviour.Bones;
        public TransformReference[] FingerBones => _fingerTrackingActorBehaviour.Bones;

        public bool RootBoneOffsetEnabled
        {
            get => _bodyTrackingActorBehaviour.RootBoneOffsetEnabled;
            set => _bodyTrackingActorBehaviour.RootBoneOffsetEnabled = value;
        }

        public HumanoidMotionActor(int actorId, string name, Animator animator)
        {
            _actorId = actorId;
            _name = name;
            _humanPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
            _humanPose.muscles = new float[HumanTrait.MuscleCount];

            var rootBoneTransform = animator.GetBoneTransform(HumanBodyBones.Hips);

            _bodyTrackingActorBehaviour = animator.gameObject.AddComponent<BodyTrackingActorBehaviour>();
            _bodyTrackingActorBehaviour.Initialize();
            _bodyTrackingActorBehaviour.UpdateRootBoneOffset(rootBoneTransform.localPosition, rootBoneTransform.localRotation);
            _bodyTrackingActorBehaviour.RootBoneOffsetEnabled = true;

            _fingerTrackingActorBehaviour = animator.gameObject.AddComponent<FingerTrackingActorBehaviour>();
            _fingerTrackingActorBehaviour.Initialize();
        }

        public void UpdateRootBoneOffset(Vector3 position, Quaternion rotation)
        {
            _bodyTrackingActorBehaviour.UpdateRootBoneOffset(position, rotation);
        }

        public void UpdateHumanPose()
        {
            _humanPoseHandler.GetHumanPose(ref _humanPose);
        }

        public void UpdateHumanPose(ref HumanPose inputData)
        {
            _humanPoseHandler.SetHumanPose(ref inputData);
            _humanPoseHandler.GetHumanPose(ref _humanPose);
        }
    }
}
