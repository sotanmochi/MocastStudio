using System;
using UnityEngine;

namespace MocastStudio.Samples.Receiver.Domain.MotionActor
{
    public sealed class HumanoidMotionActor : IDisposable
    {
        private readonly ICharacterAvatarResource _characterAvatarResource;
        private readonly HumanPoseHandler _humanPoseHandler;

        public string Name => _characterAvatarResource.Name;

        public HumanoidMotionActor(ICharacterAvatarResource characterAvatarResource)
        {
            _characterAvatarResource = characterAvatarResource;

            var animator = _characterAvatarResource.Animator;
            _humanPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
        }

        public void Dispose()
        {
            Debug.Log($"<color=orange>[{nameof(HumanoidMotionActor)}] Dispose</color>");
            _characterAvatarResource.Dispose();
        }

        public void SetHumanPose(ref HumanPose humanPose)
        {
            _humanPoseHandler.SetHumanPose(ref humanPose);
        }
    }
}
