using System.IO;
using UnityEngine;

public class ConfigManager
{
    private const string savedConfig = "savedConfig";
    private const string configFilePath = "Resources/config.json";

    public static GlobalConfig GetGlobalConfig()
    {
#if UNITY_EDITOR
        var jsonLoaded = PlayerPrefs.GetString(savedConfig, "");
        if (!string.IsNullOrEmpty(jsonLoaded))
        {
            var path = Path.Combine(Application.dataPath, configFilePath);
            var directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryName)) Directory.CreateDirectory(directoryName);
            File.WriteAllText(path, jsonLoaded);
            return JsonUtility.FromJson<GlobalConfig>(jsonLoaded);
        }
        else
        {
            Debug.LogWarning("no editor config");
            return new GlobalConfig();
        }
#else
        TextAsset defaultConfig = Resources.Load<TextAsset>("config");
        if (defaultConfig != null)
        {
            string json = defaultConfig.text;
            return JsonUtility.FromJson<GlobalConfig>(json);
        }
        else
        {
            Debug.LogError("no json in resources");
            return new GlobalConfig();
        }
#endif
    }
}