using UnityEngine;

namespace MocastStudio.Universal.Application
{
    /// <summary>
    /// Entry point
    /// </summary>
    public sealed class AppMain : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log($"[{nameof(AppMain)}] Awake()");
        }
    }
}
