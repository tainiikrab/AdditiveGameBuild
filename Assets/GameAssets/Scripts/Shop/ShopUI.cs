using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Shop
{
    public class ShopUI : MonoBehaviour
    {
        // [Header("Shop")] [SerializeField] private GameObject shop;

        [Header("Card for offers in shop")] [SerializeField]
        private GameObject offerCardPrefab;

        [Header("Place for offers")] [SerializeField]
        private Transform offersContainer;

        [Header("Drag here text from Points Label")] [SerializeField]
        private TextMeshProUGUI playerBalance;

        private ShopManager sm;

        private void Start()
        {
            sm = ShopManager.Instance;
            playerBalance.text = GameManager.Instance.points.ToString();
            sm.SetShopUI(this);
            sm.SetCategory(ShopManager.Category.Printers);
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
                // Debug.Log(item.description);
            }
        }

        private void BindOfferCard(GameObject card, ShopItemConfig item)
        {
            var shopCardUi = card.GetComponent<ShopCardUI>();
            shopCardUi.Initialize(item);
        }

        // public void SetVisible(bool visible)
        // {
        //     if (shop != null) shop.SetActive(visible);
        // }

        public void SetCategory(ShopManager.Category category)
        {
            sm.SetCategory(category);
        }
    }
}