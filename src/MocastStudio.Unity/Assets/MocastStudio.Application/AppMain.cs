using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace MocastStudio.Application
{
    /// <summary>
    /// Entry point
    /// </summary>
    public sealed class AppMain : IInitializable
    {
        private readonly List<string> _sceneNames = new List<string>
        {
            "CameraSystem",
            "SystemUIView",
            "DefaultStage",
        };

        async void IInitializable.Initialize()
        {
            foreach (var sceneName in _sceneNames)
            {
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("DefaultStage"));

            Debug.Log($"[{nameof(AppMain)}] Initialized");
        }
    }
}
