using UnityEngine;
using VContainer.Unity;

namespace MocastStudio.Samples.Receiver.Application
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
