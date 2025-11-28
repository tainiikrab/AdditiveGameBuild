using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.tvOS;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {get; private set;}
    
    private List<ShopItemConfig> purchasedItems;
    
    private List<SceneObject> purchasedSceneObjects; // Device / Interior
    
    private ShopManager sm;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;

        sm.OnPurchase += SortPurchasedOffers;
        
    }

    /// <summary>
    /// Распределяем купленные товары на объекты сцены и не объекты сцены
    /// </summary>
    private void SortPurchasedOffers()
    {
        foreach (var item in purchasedItems)
        {
            var sceneObject = ShopManager.Instance.SceneObjects.Find(x => x.SceneObjectId == item.id);
            if (sceneObject != null)
            {
                purchasedSceneObjects.Add(sceneObject);
            }
        }
    }

    public void AddItemToInventory(ShopItemConfig purchasedOffer)
    {
        purchasedItems.Add(purchasedOffer);
    }

    public ShopItemConfig GetItem(int id)
    {
        return purchasedItems[id];
    }

    public ShopItemConfig GetItem(string name)
    {
        return purchasedItems.Find(element => element.name == name);
    }

    public List<ShopItemConfig> PurchasedItems()
    {
        return purchasedItems;
    }

    public SceneObject GetSceneObject(string id)
    {
        var result = purchasedSceneObjects.Find(x => x.SceneObjectId == id);
        return result;
    }
}
