using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MocastStudio.Application
{
    public sealed class AppMain
    {
        readonly List<string> _sceneNames = new List<string>
        {
            "CameraSystem",
            "SystemUIView",
            "DefaultStage",
        };

        readonly AppSettingsRepository _appSettingsRepository;

        public AppMain(AppSettingsRepository appSettingsRepository)
        {
            _appSettingsRepository = appSettingsRepository;
        }

        public async UniTask InitializeAsync()
        {
            var loaded = await _appSettingsRepository.LoadAsync();
            if (!loaded)
            {
                _appSettingsRepository.CreateNewSettings();
            }

            var appSettings = _appSettingsRepository.AppSettings;

            var appVersion = UnityEngine.Application.version;
            if (appSettings.AppVersion != appVersion)
            {
                appSettings.AppVersion = appVersion;
                appSettings.IsUpdated = true;
            }

            foreach (var sceneName in _sceneNames)
            {
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("DefaultStage"));

            Debug.Log($"[{nameof(AppMain)}] Initialized");
        }
    }
}
