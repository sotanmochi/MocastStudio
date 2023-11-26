using UnityEngine;

namespace MocastStudio.Samples.Receiver.Application
{
    /// <summary>
    /// Entry point
    /// </summary>
    public sealed class AppMain : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log($"[{nameof(AppMain)}] Awake");
        }
    }
}
