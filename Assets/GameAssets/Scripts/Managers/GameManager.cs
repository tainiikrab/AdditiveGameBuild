using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public WayPointFollower player;

    [SerializeField] private float openMinigameDelay;

    private static GlobalConfig _globalConfig;

    public bool isUIOpened = false;

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
        _points = 2200;
    }

    private void Start()
    {
        player.StopMovement();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.H)) StartCoroutine(CreateOrdersDelayed());
        // if (Input.GetKeyDown(KeyCode.G)) player.SwitchPath(PathType.ToPrinter);
        // if (Input.GetKeyDown(KeyCode.F)) player.SwitchPath(PathType.ToLaptop);
    }

    private IEnumerator CreateOrdersDelayed()
    {
        yield return new WaitForSeconds(0);

        OrderManager.CreateRegularOrder(3);
        // OrderManager.CreatePlotOrder(currentPlotIndex);
        // currentPlotIndex++;
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

    public void OnOrderAccepted()
    {
    }

    public void OnOrderRejected()
    {
    }

    public void OnOrderComplete()
    {
    }
}