using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Universal.Application
{
    public sealed class AppMainLifecycle : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(AppMainLifecycle)}] Configure");
            builder.RegisterEntryPoint<AppMain>(Lifetime.Singleton);
            builder.RegisterMessagePipe();
        }
    }
}
