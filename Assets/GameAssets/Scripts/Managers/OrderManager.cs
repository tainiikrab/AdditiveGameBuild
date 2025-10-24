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


    private static int minOrderAmount;

    private static GameManager gm => GameManager.Instance;

    public static OrderConfig currentOrder { get; private set; }
    public static OrderQuality currentOrderQuality { get; set; }

    public static void SetCurrentOrder(OrderConfig order)
    {
        currentOrder = order;
        currentOrderQuality = new OrderQuality();
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

    public static void CreatePlotOrder(int plotIndex)
    {
        OnOrderPlotCreated?.Invoke(GetPlotOrder(plotIndex));
        SetCurrentOrder(GetPlotOrder(plotIndex));
    }

    public static OrderConfig GetPlotOrder(int plotIndex)
    {
        foreach (var OrderConfig in GlobalConfig.Instance.Orders)
            Debug.Log($"Plot index: {OrderConfig.plotIndex}");
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
        Debug.Log("Order completed");
        completedOrders.Add(currentOrder);
        availableOrders.Remove(currentOrder);
        currentOrder = null;
        OnOrderCompleted?.Invoke(currentOrder);
        OnOrderFinished?.Invoke();

        FillOrders();
    }

    public static event Action<OrderConfig> OnOrderDeclined;

    public static void DeclineOrder(OrderConfig orderConfig)
    {
        if (orderConfig == currentOrder)
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

    public class OrderQuality
    {
        public float fillDensity;
        public float layerHeight;
        public float printSpeed;
        public float sandpapering;
        public float supports;
        public float totalQuality => (layerHeight + fillDensity + printSpeed + sandpapering + supports) / 5;
    }
}