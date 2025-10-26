using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopCardUI : ShopItem
{
    [SerializeField] private Button questionButton;
    [SerializeField] private GameObject purchaseWindowPrefab;

    private ShopUI _shopUI;
    
    private void Awake()
    {
        _shopUI = GetComponentInParent<ShopUI>();
    }

    public override void Initialize(ShopItemConfig item)
    {
        base.Initialize(item);
        if (questionButton != null) questionButton.onClick.AddListener(OnClickQuestion);
    }

    private void OnClickQuestion()
    {
        var purchaseWindow = Instantiate(purchaseWindowPrefab, _shopUI.transform);
        purchaseWindow.GetComponent<PurchaseWindow>().Initialize(_itemData);
        AudioManager.Instance.PlayClickSound();
    }
}