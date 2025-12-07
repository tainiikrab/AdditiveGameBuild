using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class OrderManager
{
    public static OrderConfig currentPlotOrder;
    public static List<OrderConfig> availableOrders = new();
    public static List<OrderConfig> completedOrders = new();
    public static List<OrderConfig> declinedOrders = new();

    public static bool goPrint = false;

    private static OrderData _defaultOrder = new(GlobalConfig.Instance.Orders[0]);

    public static OrderData defaultOrder
    {
        get
        {
            Debug.LogWarning("Used default order");
            return _defaultOrder;
        }
    }

    private static int minOrderAmount;

    private static GameManager gm => GameManager.Instance;

    // private static OrderConfig currentOrder { get; set; }
    public static OrderData orderData { get; set; }
    // public static OrderQuality currentOrderQuality { get; set; }

    public static void SetCurrentOrder(OrderConfig order)
    {
        orderData = new OrderData(order);
        Debug.Log("set current order to plot oreder");
        OnOrderAccepted?.Invoke();
    }

    public static event Action OnOrderAccepted;

    public static void FillOrders()
    {
        // while (availableOrders.Count < 1) CreateRegularOrder();
    }

    public static event Action<OrderConfig> OnOrderPlotCreated;
    public static event Action<OrderConfig> OnRegularOrderCreated;


    public static void CreateRegularOrder(int amount = 1)
    {
        for (var i = 0; i < amount; i++)
        {
            var createdOrder = GetRandomAvailableOrder();
            availableOrders.Add(createdOrder);
            OnRegularOrderCreated?.Invoke(createdOrder);
        }
    }

    public static int currentPlotIndex { get; set; } = 0;

    public static void CreatePlotOrder(int plotIndex = -1)
    {
        currentPlotIndex = plotIndex == -1 ? currentPlotIndex + 1 : plotIndex;

        OnOrderPlotCreated?.Invoke(GetPlotOrder(currentPlotIndex));
        SetCurrentOrder(GetPlotOrder(currentPlotIndex));
    }

    public static OrderConfig GetPlotOrder(int plotIndex)
    {
        // foreach (var OrderConfig in GlobalConfig.Instance.Orders)
        //     Debug.Log($"Plot index: {OrderConfig.plotIndex}");
        var orderConfig =
            GlobalConfig.Instance.Orders.Find(order => order.plotIndex == plotIndex);

        if (orderConfig == null) Debug.LogError($"OrderConfig with plot index {plotIndex} is null");
        return orderConfig;
    }

    public static OrderConfig GetRandomAvailableOrder()
    {
        var orderConfig = GlobalConfig.Instance.Orders
            .Where(o => o.isMail)
            .OrderBy(_ => Random.value)
            .FirstOrDefault();
        if (orderConfig == null) Debug.LogError("No available orders found");
        return orderConfig;
    }

    public static event Action<OrderConfig> OnOrderCompleted;
    public static event Action OnOrderFinished;

    public static void CompleteOrder()
    {
        if (orderData == null)
        {
            Debug.LogWarning("Order is null");
            return;
        }


        Debug.Log("Order completed");
        completedOrders.Add(orderData.config);
        availableOrders.Remove(orderData.config);
        OnOrderCompleted?.Invoke(orderData.config);
        orderData = null;
        OnOrderFinished?.Invoke();

        FillOrders();
    }

    public static event Action<OrderConfig> OnOrderDeclined;

    public static void DeclineOrder(OrderConfig orderConfig)
    {
        if (orderData == null)
        {
            Debug.LogWarning("Order is null");
            return;
        }

        if (orderConfig == orderData.config)
        {
            Debug.Log("Cannot decline current order");
            return;
        }

        Debug.Log("Order declined");
        OnOrderDeclined?.Invoke(orderConfig);
        declinedOrders.Add(orderConfig);
        availableOrders.Remove(orderConfig);

        FillOrders();
    }

    [Serializable]
    public class OrderQuality
    {
        public float fillDensity { get; set; }
        public float layerHeight { get; set; }
        public float printSpeed { get; set; }
        public float sandpapering { get; set; }
        public float supports { get; set; }

        public float totalQuality
        {
            get
            {
                var values = GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(float) && p.Name != nameof(totalQuality))
                    .Select(p => (float)p.GetValue(this));

                return values.Any() ? values.Average() : 0;
            }
        }
    }

    [Serializable]
    public class OrderData
    {
        public OrderData(OrderConfig orderConfig)
        {
            config = orderConfig;
        }

        public OrderQuality quality = new();
        public OrderConfig config;
        public int currentMinigame = -1;
        public PrintingMaterialConfig chosenMaterial;

        public void LoadNextMinigame()
        {
            currentMinigame++;
            MinigameManager.Instance.OpenMinigame(config.printerType.minigames[currentMinigame]);
            Debug.Log($"The next minigame is {config.printerType.minigames[currentMinigame]}");
            Debug.Log($"Minigame index is {currentMinigame}");
        }
    }
}