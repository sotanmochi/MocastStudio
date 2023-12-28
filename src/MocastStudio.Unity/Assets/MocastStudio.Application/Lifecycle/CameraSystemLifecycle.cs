using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class CameraSystemLifecycle : LifetimeScope
    {
        [Header("Components")]
        [SerializeField] CameraSystemEntryPoint _cameraSystemEntryPoint;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(CameraSystemLifecycle)}] Configure");

            builder.RegisterComponent(_cameraSystemEntryPoint);
        }
    }
}
