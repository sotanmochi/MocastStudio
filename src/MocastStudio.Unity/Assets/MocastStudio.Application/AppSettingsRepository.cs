using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace MocastStudio.Application
{
    public sealed class AppSettingsRepository : IDisposable
    {
        readonly string _filepath;

        AppSettings _appSettings;

        public AppSettings AppSettings => _appSettings;

        public AppSettingsRepository(string directoryPath, string filename)
        {
            directoryPath = string.IsNullOrEmpty(directoryPath) ? UnityEngine.Application.persistentDataPath : directoryPath;
            _filepath = Path.Combine(directoryPath, filename);
        }

        public void Dispose()
        {
            if (_appSettings.IsUpdated)
            {
                SaveAsync();
            }
        }

        public void CreateNewSettings()
        {
            var clientId = PlayerPrefs.HasKey(PlayerPrefsKeys.ClientId)
                ? Ulid.Parse(PlayerPrefs.GetString(PlayerPrefsKeys.ClientId))
                : Ulid.NewUlid();

            _appSettings = new AppSettings()
            {
                AppName = UnityEngine.Application.productName,
                AppVersion = UnityEngine.Application.version,
                ClientId = clientId,
                IsUpdated = true,
            };
        }

        public async Task<bool> LoadAsync()
        {
            var loaded = false;

            try
            {
                var json = await File.ReadAllTextAsync(_filepath);
                _appSettings = JsonConvert.DeserializeObject<AppSettings>(json);

                if (PlayerPrefs.HasKey(PlayerPrefsKeys.ClientId))
                {
                    var clientId = PlayerPrefs.GetString(PlayerPrefsKeys.ClientId);
                    _appSettings.ClientId = Ulid.Parse(clientId);
                }
                else
                {
                    _appSettings.ClientId = Ulid.NewUlid();
                    _appSettings.IsUpdated = true;
                }

                loaded = true;
            }
            catch (Exception e)
            {
                // Debug.LogError(e);
            }

            return loaded;
        }

        public async Task<bool> SaveAsync()
        {
            var saved = false;

            try
            {
                SavePlayerPrefs();

                var json = JsonConvert.SerializeObject(_appSettings);
                await File.WriteAllTextAsync(_filepath, json);

                saved = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return saved;
        }

        void SavePlayerPrefs()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.ClientId))
            {
                PlayerPrefs.SetString(PlayerPrefsKeys.ClientId, _appSettings.ClientId.ToString());
            }

            PlayerPrefs.Save();
        }

        static class PlayerPrefsKeys
        {
            public readonly static string ClientId = "AppSettings.ClientId";
        }
    }
}
