using System.Collections.Generic;
using System.Linq;
using _Scripts.Shop;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<ShopItemConfig> inventory;

    private GameManager gm;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        inventory = new List<ShopItemConfig>();
        DontDestroyOnLoad(gameObject);

        foreach (var item in SaveManager.gameData.purchasedOffers) AddToInventory(GetShopItemById(item));
    }

    public void Start()
    {
        gm = GameManager.Instance;
    }

    public void AddToInventory(ShopItemConfig item)
    {
        inventory.Add(item);
        Debug.Log("В инвентаре - " + inventory.Count);
    }

    private ShopItemConfig GetShopItemById(string id)
    {
        var offer = gm.globalConfig.ShopItems
            .FirstOrDefault(o => o.id == id);

        // Debug.Log($"Got offer: {offer?.id}");
        if (offer != null)
            return offer;
        Debug.LogWarning($"Offer with id {id} not found in config");
        return null;
    }

    public bool HasItem(ShopItemConfig item)
    {
        return inventory.Contains(item);
    }
}