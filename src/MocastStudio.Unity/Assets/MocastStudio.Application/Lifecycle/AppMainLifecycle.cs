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

            builder.RegisterMessagePipe();

            builder.RegisterEntryPoint<AppMain>(Lifetime.Singleton);

            builder.Register<AppSettingsRepository>(Lifetime.Singleton)
                .WithParameter("directoryPath", UnityEngine.Application.persistentDataPath)
                .WithParameter("filename", "appsettings.json");

            builder.Register<CameraSystemContext>(Lifetime.Singleton);
        }
    }
}
