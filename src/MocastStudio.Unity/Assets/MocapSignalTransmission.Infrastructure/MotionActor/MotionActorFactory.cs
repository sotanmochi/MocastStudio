using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MocapSignalTransmission.BinaryDataProvider;
using MocapSignalTransmission.MotionActor;

namespace MocapSignalTransmission.Infrastructure.MotionActor
{
    public sealed class MotionActorFactory : IMotionActorFactory
    {
        private readonly ICharacterAvatarResourceProvider _characterAvatarResourceProvider;

        public MotionActorFactory(ICharacterAvatarResourceProvider characterAvatarResourceProvider)
        {
            _characterAvatarResourceProvider = characterAvatarResourceProvider;
        }

        public async Task<HumanoidMotionActor> CreateAsync(int actorId, IBinaryDataLoadingRequest request, CancellationToken cancellationToken = default)
        {
            var characterAvatarResource = await _characterAvatarResourceProvider.LoadAsync(request, cancellationToken);

            if (characterAvatarResource is VrmAvatar vrmAvatar)
            {
                vrmAvatar.ShowMeshes();
                await UniTask.DelayFrame(1); // NOTE: Wait for ControlRig to be applied.
            }

            return new HumanoidMotionActor(actorId, characterAvatarResource.Name, characterAvatarResource.Animator);
        }
    }
}
