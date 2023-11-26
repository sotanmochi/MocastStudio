using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Samples.Receiver.Application
{
    public sealed class AppMainLifecycle : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(AppMainLifecycle)}] Configure");
            builder.RegisterEntryPoint<AppMain>(Lifetime.Singleton);
        }
    }
}
