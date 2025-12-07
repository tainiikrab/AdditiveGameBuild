using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Rellac.Windows;
using UnityEngine;
using UnityEngine.UI;

public class MailUI : MonoBehaviour
{
    public enum Category
    {
        Incoming,
        Completed,
        Special,
        Declined
    }

    public static MailUI Instance;

    public MailHeaderUI activeHeader;

    [Header("Holders")]
    // [SerializeField] private RectTransform headerContainer;
    [SerializeField]
    private CategoryContainer[] containers;

    [SerializeField] private RectTransform bodyContainer;

    [Header("Prefabs")] [SerializeField] private MailHeaderUI mailHeaderPrefab;
    [SerializeField] private GUIWindow slicerWindow;

    [Header("Misc")] [SerializeField] private Color normalColor = Color.white;

    [SerializeField] private Color activeColor = Color.green;

    private readonly List<MailHeaderUI> headers = new();
    private Dictionary<Category, Transform> categoryRoots;

    public static OrderConfig currentOrder
    {
        get
        {
            if (OrderManager.orderData == null) return null;
            return OrderManager.orderData.config;
        }
    }

    public static void AcceptOrder(OrderConfig orderConfig, MailHeaderUI header)
    {
        OrderManager.SetCurrentOrder(orderConfig);
        Instance.MoveHeaderToCategory(header, Category.Completed);
        SaveManager.gameData.completedOrders.Add(orderConfig.id);
       //AudioManager.Instance.PlaySound(SoundType.Accept);

        OrderManager.orderData.LoadNextMinigame();
        Instance.GetComponent<GUIWindow>().CloseWindow();
        //AudioManager.Instance.PlaySound(SoundType.Accept);
    }

    public static void DeclineOrder(OrderConfig orderConfig, MailHeaderUI header)
    {
        OrderManager.DeclineOrder(orderConfig);
        Instance.MoveHeaderToCategory(header, Category.Declined);

        SaveManager.gameData.declinedOrders.Add(orderConfig.id);
        Debug.Log($"Declined order: {orderConfig.id}");

        AudioManager.Instance.PlaySound(SoundType.Cancel);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        foreach (var c in containers)
            for (var i = 0; i < c.contentRoot.childCount; i++)
                Destroy(c.contentRoot.GetChild(i).gameObject);

        InitializeCategories();
        Instance = this;
        foreach (Transform child in bodyContainer) Destroy(child.gameObject);
        ReloadUI();
        OrderManager.OnRegularOrderCreated += AddOrder;
        // OrderManager.OnOrderCompleted += RemoveOrder;
        // OrderManager.OnOrderDeclined += RemoveOrder;
    }

    private void OnDestroy()
    {
        OrderManager.OnRegularOrderCreated -= AddOrder;
        // OrderManager.OnOrderCompleted -= RemoveOrder;
        // OrderManager.OnOrderDeclined -= RemoveOrder;
    }

    private void InitializeCategories()
    {
        if (categoryRoots == null) categoryRoots = containers.ToDictionary(c => c.category, c => c.contentRoot);

        foreach (var c in containers)
        {
            var captured = c.category; // без этого не робит
            c.button.onClick.AddListener(() => SwitchCategory(captured));
        }

        SwitchCategory(Category.Incoming);
    }

    private void SwitchCategory(Category category)
    {
        foreach (var c in containers)
        {
            var isActive = c.category == category;

            var cg = c.contentRoot.GetComponent<CanvasGroup>();
            if (cg == null) cg = c.contentRoot.gameObject.AddComponent<CanvasGroup>();

            if (isActive)
            {
                c.contentRoot.gameObject.SetActive(true);
                cg.alpha = 0f;
                cg.DOFade(1f, 0.1f).SetEase(Ease.OutQuad);
                cg.transform.localScale = Vector3.one * 0.95f;
                cg.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
            }
            else
            {
                cg.DOFade(0f, 0.1f).OnComplete(() => { c.contentRoot.gameObject.SetActive(false); });
            }

            var colors = c.button.colors;
            var targetColor = isActive ? activeColor : normalColor;
            var img = c.button.GetComponent<Image>();
            if (img != null)
            {
                img.DOColor(targetColor, 0.1f);
            }
            else
            {
                colors.normalColor = targetColor;
                c.button.colors = colors;
            }
        }

        //AudioManager.Instance.PlaySound(SoundType.Switch);
    }

    public void MoveHeaderToCategory(MailHeaderUI header, Category newCategory)
    {
        if (categoryRoots.TryGetValue(newCategory, out var root))
        {
            header.transform.SetParent(root, false);
            header.category = newCategory;
        }
        else
        {
            Debug.LogError($"No root found for category {newCategory}");
        }
    }

    public void ActivateHeader(MailHeaderUI header)
    {
        if (activeHeader == null)
        {
            header.Toggle(true);
            activeHeader = header;
            return;
        }

        if (activeHeader == header) return;
        activeHeader.Toggle(false);
        header.Toggle(true);
        activeHeader = header;
    }

    private void InitializeUI()
    {
        OrderManager.FillOrders();
        foreach (var order in OrderManager.availableOrders)
            // Debug.Log($"Order is null = {order == null}");
            AddOrder(order);

        foreach (var completedOrder in SaveManager.gameData.completedOrders)
            AddOrder(GetOrderById(completedOrder), Category.Completed);
        foreach (var declinedOrder in SaveManager.gameData.declinedOrders)
            AddOrder(GetOrderById(declinedOrder), Category.Declined);
        // if (headers.Count != 0) ActivateHeader(headers[0]);
    }

    private OrderConfig GetOrderById(string id)
    {
        var order = GlobalConfig.Instance.Orders
            .FirstOrDefault(o => o.id == id);

        // Debug.Log($"Got order: {order?.id}");
        if (order != null)
            return order;
        Debug.LogWarning($"Order with id {id} not found in config");
        return null;
    }


    private void ReloadUI()
    {
        foreach (var header in headers) Destroy(header.gameObject);
        headers.Clear();
        InitializeUI();
    }

    private void AddOrder(OrderConfig order)
    {
        AddOrder(order, Category.Incoming);
        //AudioManager.Instance.PlayClickSound();
    }

    private void AddOrder(OrderConfig order, Category category)
    {
        var orderHeader = Instantiate(mailHeaderPrefab, categoryRoots[category]);
        // Debug.Log($"Adding order {order.id} to category {category}");
        orderHeader.category = category;
        orderHeader.Initialize(order, bodyContainer);
        orderHeader.Toggle(false);
        headers.Add(orderHeader);

        //AudioManager.Instance.PlayClickSound();
        // ActivateHeader(orderHeader);
    }

    // public void OpenSlicer()
    // {
    //     var window = Instantiate(slicerWindow, transform.parent);
    //     GetComponent<GUIWindow>().CloseWindow();
    //
    //     AudioManager.Instance.PlayClickSound();
    // }
// public void RemoveOrder(OrderConfig order)
// {
//     foreach (var header in headers)
//     {
//         if (header.orderConfig != order)
//             continue;
//
//         Destroy(header.gameObject);
//
//         headers.Remove(header);
//
//         break;
//     }
// }

    [Serializable]
    public struct CategoryContainer
    {
        public Category category;
        public Transform contentRoot;
        public Button button;
    }
}