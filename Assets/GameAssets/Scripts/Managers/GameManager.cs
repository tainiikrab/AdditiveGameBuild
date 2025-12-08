using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public WayPointFollower player;
    public Printer printer;

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

        Instance = this;
        globalConfig = GlobalConfig.Instance;
        _points = 2200;
    }

    private void Start()
    {
        player.StopMovement();
        if (OrderManager.goPrint)
        {
            if (SceneSwitchManager.areMinigamesFinished) return;
            SetupPrinterPathActions();
            // printer.defaultModel = OrderManager.orderData.config.mesh;
            player.StartMovement();
            StartCoroutine(SwitchToNextMinigameAfterDelay());
        }
    }

    [Space(10)] [SerializeField] private float switchToNextMinigameDelay = 0.5f;

    private IEnumerator SwitchToNextMinigameAfterDelay()
    {
        yield return new WaitForSeconds(switchToNextMinigameDelay);
        OrderManager.orderData.LoadNextMinigame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            OrderManager.CreatePlotOrder(1);
            OrderManager.orderData.currentMinigame = 0;
            Debug.Log("Plot order created");
            Debug.Log(OrderManager.orderData.ToString());
            Debug.Log(OrderManager.orderData.config.orderName);
            Debug.Log(OrderManager.orderData.config.mesh);
            Debug.Log(OrderManager.orderData.config.printerType.minigames[0]);
            Debug.Log(OrderManager.orderData.config.printerType.minigames[1]);
        }
    }

    public void SendPlayerToPrinter()
    {
        player.SwitchPath(PathType.ToPrinter);
    }

    private IEnumerator CreateOrdersDelayed()
    {
        yield return new WaitForSeconds(0);

        // OrderManager.CreateRegularOrder(3);
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

    public void OnDialogueFinished()
    {
        // OrderManager.orderData.LoadNextMinigame();
        // player.StartMovement();
    }


    public void OnOrderRejected()
    {
    }

    public void OnOrderComplete()
    {
    }

    private void SetupPrinterPathActions()
    {
        for (var i = 0; i < player.Paths.Length; i++)
            if (player.Paths[i].Type == PathType.ToPrinter)
            {
                player.ClearOnPathEndActions(i);

                player.AddOnPathEndAction(i, () => { StartCoroutine(printer.PrintHeadMoveRoutine()); });

                Debug.Log($"Setup printer actions for path index: {i}");
                break;
            }
    }
}