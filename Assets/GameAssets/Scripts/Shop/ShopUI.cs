using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopUI : MonoBehaviour
{
    // [Header("Shop")] [SerializeField] private GameObject shop;

    [Header("Card for offers in shop")] [SerializeField]
    private GameObject offerCardPrefab;

    [Header("Place for offers")] [SerializeField]
    private Transform offersContainer;

    [Header("Drag here text from Points Label")] [SerializeField]
    private TextMeshProUGUI playerBalance;
    
    [Space(20), SerializeField] private CategorySwitcher[] categorySwitchers;

    private ShopManager sm;

    private void Start()
    {
        InitCategoryButtons();
        sm = ShopManager.Instance;
        playerBalance.text = GameManager.Instance.points.ToString();
        sm.SetShopUI(this);
        sm.SetCategory(ShopManager.Category.Printers);
    }
    
    private void InitCategoryButtons()
    {
        foreach (var categorySwitcher in categorySwitchers)
        {
            categorySwitcher.button.onClick.AddListener((() => SetCategory(categorySwitcher.category)));
        }
    }
    
    public void ShowOffers(ShopItemConfig[] items)
    {
        if (offersContainer == null || offerCardPrefab == null) return;

        for (var i = offersContainer.childCount - 1; i >= 0; i--) Destroy(offersContainer.GetChild(i).gameObject);

        if (items == null) return;

        foreach (var item in items)
        {
            var card = Instantiate(offerCardPrefab, offersContainer);
            BindOfferCard(card, item);
        }
    }

    private void BindOfferCard(GameObject card, ShopItemConfig item)
    {
        var shopCardUi = card.GetComponent<ShopCardUI>();
        shopCardUi.Initialize(item);
    }

    public void SetCategory(ShopManager.Category category)
    {
        sm.SetCategory(category);
    }
    
    [Serializable]
    private struct CategorySwitcher
    {
        public ShopManager.Category category;
        public Button button;
    }
}