using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Universal.Application
{
    public sealed class AppMain : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            Debug.Log($"[{nameof(AppMain)}] Configure");
        }
    }
}
