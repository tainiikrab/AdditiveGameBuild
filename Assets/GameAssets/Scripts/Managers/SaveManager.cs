using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager
{
    private const string savedConfig = "savedConfig";
    private const string savedGameData = "savedGameData";

    private static SaveManager instance;

    private static GameData _gameData;

    private static bool deleteSaves;

    public static GameData gameData
    {
        get
        {
            if (_gameData == null) _gameData = Load();
            // if (_gameData.declinedOrders.Count > 0)
            //     Debug.Log($"Loaded game data: {_gameData.declinedOrders[0]}");
            return _gameData;
        }
        set
        {
            _gameData = value;
            Save();
        }
    }


    public static void Save()
    {
        if (deleteSaves) return;
        PlayerPrefs.SetString(savedGameData, JsonConvert.SerializeObject(gameData));
    }

    public static GameData Load()
    {
        try
        {
            var jsonLoaded = PlayerPrefs.GetString(savedGameData);
            if (string.IsNullOrEmpty(jsonLoaded))
                return new GameData();

            return JsonConvert.DeserializeObject<GameData>(jsonLoaded) ?? new GameData();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load GameData: {e}");
            return new GameData();
        }
    }

    public static void DeleteSaves()
    {
        // OrderManager.CompleteOrder();
        // OrderManager.availableOrders = new List<OrderConfig>();
        gameData = new GameData();
        PlayerPrefs.DeleteKey(savedGameData);
        deleteSaves = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // public static GlobalConfig GetGlobalConfig()
    // {
    //     var jsonLoaded = PlayerPrefs.GetString(savedConfig);
    //     GlobalConfig globalConfig;
    //     if (!string.IsNullOrEmpty(jsonLoaded))
    //     {
    //         globalConfig = JsonUtility.FromJson<GlobalConfig>(jsonLoaded);
    //     }
    //     else
    //     {
    //         globalConfig = new GlobalConfig();
    //         Debug.LogWarning("No saved config found");
    //     }
    //
    //     // foreach (var order in globalConfig.Orders) Debug.Log(order.id);
    //     return globalConfig;
    // }
    public static GlobalConfig GetGlobalConfig()
    {
#if UNITY_EDITOR
        var jsonLoaded = PlayerPrefs.GetString(savedConfig, "");
        if (!string.IsNullOrEmpty(jsonLoaded))
        {
            var path = Path.Combine(Application.dataPath, "Resources/config.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
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

    [Serializable]
    public class GameData
    {
        public int points;
        public List<string> completedOrders = new();
        public List<string> declinedOrders = new();
        public List<string> purchasedOffers = new();
    }
}