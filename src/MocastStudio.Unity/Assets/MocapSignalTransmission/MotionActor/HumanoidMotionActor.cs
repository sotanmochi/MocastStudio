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

        public HumanoidMotionActor(int actorId, string name, Animator animator)
        {
            _actorId = actorId;
            _name = name;
            _humanPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);

            _bodyTrackingActorBehaviour = animator.gameObject.AddComponent<BodyTrackingActorBehaviour>();
            _bodyTrackingActorBehaviour.Initialize();

            _fingerTrackingActorBehaviour = animator.gameObject.AddComponent<FingerTrackingActorBehaviour>();
            _fingerTrackingActorBehaviour.Initialize();
        }

        public void UpdateHumanPose()
        {
            _humanPoseHandler.GetHumanPose(ref _humanPose);
        }
    }
}
