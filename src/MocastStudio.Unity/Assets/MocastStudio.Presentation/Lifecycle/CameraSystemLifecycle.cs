using MocastStudio.Presentation.CameraSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Presentation.Lifecycle
{
    public sealed class CameraSystemLifecycle : LifetimeScope
    {
        [Header("CameraSystem Components")]
        [SerializeField] CameraSwitcher _cameraSwitcher;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(CameraSystemLifecycle)}] Configure");
            builder.RegisterComponent(_cameraSwitcher);
            builder.RegisterEntryPoint<CameraSystemPresenter>();
        }
    }
}
