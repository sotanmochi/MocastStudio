using VContainer;
using VContainer.Unity;
using CameraSystemContext = MocastStudio.CameraSystem.CameraSystemContext;
using Debug = UnityEngine.Debug;

namespace MocastStudio.Application.Lifecycle
{
    public sealed class AppMainLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(AppMainLifetimeScope)}] Configure");

            builder.RegisterEntryPoint<AppMainLifecycle>();

            builder.Register<AppMain>(Lifetime.Singleton);
            builder.Register<AppSettingsRepository>(Lifetime.Singleton)
                .WithParameter("directoryPath", UnityEngine.Application.persistentDataPath)
                .WithParameter("filename", "appsettings.json");

            builder.Register<CameraSystemContext>(Lifetime.Singleton);
        }
    }
}
