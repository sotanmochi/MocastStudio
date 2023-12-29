using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

namespace MocastStudio.Application
{
    class AppInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            RegisterResolvers();
            Debug.Log($"<color=lime>[{nameof(AppInitializer)}] Initialized</color>");
        }

        static void RegisterResolvers()
        {
            Debug.Log($"<color=lime>[{nameof(AppInitializer)}] RegisterResolvers</color>");

            // NOTE:
            // Currently, CompositeResolver doesn't work on Unity IL2CPP build.
            // Use StaticCompositeResolver instead of it.
            StaticCompositeResolver.Instance.Register(
                MocapSignalTransmissionGeneratedResolver.Instance,
                SignalStreamingGeneratedResolver.Instance,
                BuiltinResolver.Instance,
                PrimitiveObjectResolver.Instance,
                StandardResolver.Instance
            );

            MessagePackSerializer.DefaultOptions =
                MessagePackSerializer.DefaultOptions.WithResolver(StaticCompositeResolver.Instance);
        }
    }
}
