using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Shop
{
    public class ShopManager : MonoBehaviour
    {
        public enum Category
        {
            Printers,
            Materials,
            Interior,
            Tools,
            None
        }

        private ShopUI shopUI;

        private Dictionary<string, ShopItemConfig[]> _sortedOffers;
        private SceneObject[] _sceneObjects;

        private Category _currentCategory = Category.None;
        private bool _parsed;

        private GameManager gm;

        public static ShopManager Instance { get; private set; }

        private void Awake()
        {
            _sceneObjects = FindObjectsByType<SceneObject>(FindObjectsSortMode.None);

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


        private void Start()
        {
            gm = GameManager.Instance;
            ParseOffers();
        }

        public void SetShopUI(ShopUI ui)
        {
            shopUI = ui;
        }

        public void Purchase(ShopItemConfig offer)
        {
            if (InventoryManager.Instance.HasItem(offer)) return;

            gm.points -= offer.price;
            InventoryManager.Instance.AddToInventory(offer);

            if (offer.category == "Interior" || offer.category == "Printers")
            {
                var sceneObject = Array.Find(_sceneObjects, item => item.gameObject.name == offer.id);
                sceneObject.gameObject.SetActive(true);
                Debug.Log(sceneObject.gameObject.name);
            }

            Debug.Log("Осталось " + gm.points);

            SaveManager.gameData.purchasedOffers.Add(offer.id);
        }

        private void ParseOffers()
        {
            //var config = SaveManager.LoadConfig();
            // Debug.Log(gm);
            // Debug.Log(gm.globalConfig);
            // Debug.Log(gm.globalConfig.ShopItems);
            var config = gm.globalConfig.ShopItems;

            var items = config ?? new List<ShopItemConfig>();

            _sortedOffers = items
                .GroupBy(
                    i => i.category.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToArray(), StringComparer.OrdinalIgnoreCase);

            _sortedOffers[""] = items.ToArray();
        }

        public void SetCategory(Category category)
        {
            _currentCategory = category;

            if (_sortedOffers == null || !_sortedOffers.TryGetValue(_currentCategory.ToString(), out var items))
                items = Array.Empty<ShopItemConfig>();
            Debug.Log(items[0].id);
            shopUI.ShowOffers(items);
            AudioManager.Instance.PlayClickSound();
        }
    }
}