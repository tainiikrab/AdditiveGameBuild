using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject offerCardPrefab;
    [SerializeField] private Transform offersContainer;
    [SerializeField] private TextMeshProUGUI playerBalance;
    //[SerializeField] private TextMeshProUGUI categoryTitle;
    
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
            categorySwitcher.button.onClick.AddListener((() => SetCategory(
                categorySwitcher.category, categorySwitcher.title
                )));
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

    private void SetCategory(ShopManager.Category category, string title = "")
    {
        sm.SetCategory(category);
        //categoryTitle.text = title;
    }
    
    [Serializable]
    private struct CategorySwitcher
    {
        public ShopManager.Category category;
        public Button button;
        public string title;
    }
}