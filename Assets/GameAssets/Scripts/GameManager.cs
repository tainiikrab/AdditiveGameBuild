using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public int currentPlotIndex = 1;

    // public WayPointFollower player;
    public OpenWindowButton slicerIcon;

    [SerializeField] private float openMinigameDelay;
    public bool isLaptopOpen;


    private bool waitsForPlayerToComeToPrinter;

    private static GlobalConfig _globalConfig;

    public GlobalConfig globalConfig
    {
        get
        {
            if (_globalConfig == null) _globalConfig = SaveManager.GetGlobalConfig();
            return _globalConfig;
        }
        private set => _globalConfig = value;
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

        Instance = this;
        globalConfig = SaveManager.GetGlobalConfig();
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


    public event Action<bool> OnLaptopToggle;

    public void ToggleLaptop(bool toggle)
    {
        if (toggle)
        {
            OnLaptopToggle?.Invoke(true);
            isLaptopOpen = true;
        }
        else
        {
            OnLaptopToggle?.Invoke(false);
            isLaptopOpen = false;
        }
    }
}