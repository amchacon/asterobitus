using System.IO;
using AsteroidsModern.Core;
using AsteroidsModern.Extensions;
using UnityEngine;

namespace AsteroidsModern.Managers
{
    public class ConfigLoader : Singleton<ConfigLoader>
    {
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private string configFileName = "game_config.json";

        public GameSettings GameSettings => gameSettings;
        
        private void Awake()
        {
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                string configPath = Path.Combine(Application.streamingAssetsPath, configFileName);
                
                if (!File.Exists(configPath))
                {
                    CreateDefaultConfig(configPath);
                }

                string jsonContent = File.ReadAllText(configPath);
                gameSettings = JsonUtility.FromJson<GameSettings>(jsonContent);

                if (gameSettings == null)
                {
                    Debug.LogWarning("ConfigLoader: Fail to parse configuration file. Using default settings.");
                    gameSettings = new GameSettings();
                }
                
                Debug.Log("ConfigLoader: Load configuration successful.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ConfigLoader: Error loading configuration file: {ex.Message}");
                gameSettings = new GameSettings();
            }
        }

        private void CreateDefaultConfig(string configPath)
        {
            try
            {
                string directory = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directory))
                {
                    if (directory != null) Directory.CreateDirectory(directory);
                }

                GameSettings defaultSettings = new GameSettings();
                string jsonContent = JsonUtility.ToJson(defaultSettings, true);
                File.WriteAllText(configPath, jsonContent);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ConfigLoader: Error to create default config file {ex.Message}");
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                string configPath = Path.Combine(Application.streamingAssetsPath, configFileName);
                string jsonContent = JsonUtility.ToJson(gameSettings, true);
                File.WriteAllText(configPath, jsonContent);
                Debug.Log("ConfigLoader: Congigurations saved successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ConfigLoader: Error to save configurations: {ex.Message}");
            }
        }

        public void ReloadConfiguration()
        {
            LoadConfiguration();
        }
    }
}