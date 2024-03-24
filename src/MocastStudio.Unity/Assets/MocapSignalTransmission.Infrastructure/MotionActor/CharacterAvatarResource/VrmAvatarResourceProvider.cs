using System.Threading;
using System.Threading.Tasks;
using MocapSignalTransmission.BinaryDataProvider;
using MocapSignalTransmission.Infrastructure.Constants;
using UniGLTF;
using UniVRM10;

namespace MocapSignalTransmission.Infrastructure.MotionActor
{
    public sealed class VrmAvatarResourceProvider : ICharacterAvatarResourceProvider
    {
        private readonly RenderPipelineType _renderPipelineType;
        private readonly IBinaryDataProvider _binaryDataProvider;

        public VrmAvatarResourceProvider(RenderPipelineType renderPipelineType, IBinaryDataProvider binaryDataProvider)
        {
            _renderPipelineType = renderPipelineType;
            _binaryDataProvider = binaryDataProvider;
        }

        public async Task<ICharacterAvatarResource> LoadAsync<T>(T request, CancellationToken cancellationToken = default) where T : IBinaryDataLoadingRequest
        {
            var bytes = await _binaryDataProvider.LoadAsync(request, cancellationToken);
            if (bytes == null || bytes.Length == 0) return null;

            IMaterialDescriptorGenerator materialGenerator = _renderPipelineType switch
            {
                RenderPipelineType.BuiltIn => new BuiltInVrm10MaterialDescriptorGenerator(),
                RenderPipelineType.UniversalRenderPipeline => new UrpVrm10MaterialDescriptorGenerator(),
                _ => new BuiltInVrm10MaterialDescriptorGenerator()
            };

            var loadedVrm = await Vrm10.LoadBytesAsync(bytes,
                canLoadVrm0X: true,
                showMeshes: false,
                materialGenerator: materialGenerator,
                ct: cancellationToken);

            if (loadedVrm == null) return null;

            return new VrmAvatar(loadedVrm.GetComponent<RuntimeGltfInstance>(), loadedVrm.Vrm.Meta.Name);
        }
    }
}
