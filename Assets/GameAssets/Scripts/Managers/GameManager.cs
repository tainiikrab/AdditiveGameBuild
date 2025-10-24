using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentPlotIndex = 1;

    [SerializeField] private float openMinigameDelay;

    private static GlobalConfig _globalConfig;

    private GlobalConfig globalConfig
    {
        get
        {
            if (_globalConfig == null) _globalConfig = GlobalConfig.Instance;
            return _globalConfig;
        }
        set => _globalConfig = value;
    }

    private int _points;

    public event Action<int> OnPointsChanged;

    public int points
    {
        get => _points;
        set
        {
            _points = value;
            OnPointsChanged?.Invoke(value);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Debug.Log("Instance of GameManager");
        Instance = this;
        globalConfig = GlobalConfig.Instance;
    }

    private void OnDestroy()
    {
        Instance = null;
        SaveManager.gameData.points = points;
    }

    private void OnApplicationQuit()
    {
        SaveManager.Save();
    }
}