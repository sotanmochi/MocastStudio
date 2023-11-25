using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Universal.Application
{
    /// <summary>
    /// Entry point
    /// </summary>
    public sealed class AppMain : IInitializable
    {
        void IInitializable.Initialize()
        {
            Debug.Log($"[{nameof(AppMain)}] Initialize");
        }
    }
}
