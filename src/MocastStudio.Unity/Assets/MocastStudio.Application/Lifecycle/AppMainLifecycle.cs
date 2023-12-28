using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using CameraSystemContext = MocastStudio.CameraSystem.CameraSystemContext;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class AppMainLifecycle : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(AppMainLifecycle)}] Configure");

            builder.RegisterEntryPoint<AppMain>(Lifetime.Singleton);
            builder.RegisterMessagePipe();

            builder.Register<CameraSystemContext>(Lifetime.Singleton);
        }
    }
}
