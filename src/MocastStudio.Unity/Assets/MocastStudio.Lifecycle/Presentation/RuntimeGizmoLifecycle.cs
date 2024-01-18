using MocastStudio.Presentation.UserInteraction.RuntimeGizmo;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Lifecycle.Presentation
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
