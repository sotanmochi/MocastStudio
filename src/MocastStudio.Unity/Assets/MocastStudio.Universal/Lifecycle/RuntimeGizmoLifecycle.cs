using MocastStudio.Universal.UserInteraction.RuntimeGizmo;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Universal.Lifecycle
{
    public sealed class RuntimeGizmoLifecycle : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(RuntimeGizmoLifecycle)}] Configure");

            builder.RegisterEntryPoint<MotionActorReferencePointControl>();
        }
    }
}
