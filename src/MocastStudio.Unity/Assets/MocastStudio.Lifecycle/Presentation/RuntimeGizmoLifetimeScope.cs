using MocastStudio.Presentation.UserInteraction.RuntimeGizmo;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Lifecycle.Presentation
{
    public sealed class RuntimeGizmoLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(RuntimeGizmoLifetimeScope)}] Configure");

            builder.RegisterEntryPoint<MotionActorReferencePointControl>();
        }
    }
}
