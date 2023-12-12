using MocastStudio.CameraSystem;
using UnityEngine;
using VContainer;

namespace MocastStudio.Universal.Lifecycle
{
    public sealed class CameraSystemEntryPoint : MonoBehaviour
    {
        [SerializeField] Camera _sceneViewCamera;

        CameraSystemContext _cameraSystemContext;

        [Inject]
        public void Construct(CameraSystemContext cameraSystemContext)
        {
            _cameraSystemContext = cameraSystemContext;
        }
 
        void Awake()
        {
            _cameraSystemContext.SceneViewCamera = _sceneViewCamera;
        }
    }
}
