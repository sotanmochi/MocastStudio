using MocastStudio.Presentation.CameraSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Lifecycle.Presentation
{
    public sealed class CameraSystemLifetimeScope : LifetimeScope
    {
        [Header("CameraSystem Components")]
        [SerializeField] CameraSystemLifecycle _cameraSystemLifecycle;
        [SerializeField] CameraSwitcher _cameraSwitcher;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(CameraSystemLifetimeScope)}] Configure");

            builder.RegisterComponent(_cameraSystemLifecycle);
            builder.RegisterComponent(_cameraSwitcher);

            builder.RegisterEntryPoint<CameraSystemPresenter>();
        }
    }
}
