using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Transform offersContainer;
    [SerializeField] private OfferCardUI offerCard;
    [SerializeField] private TextMeshProUGUI categoryTitle;
    [SerializeField] private TextMeshProUGUI pointsLabelText;
    [SerializeField] private RectTransform scrollbarGroup;
    [SerializeField] private ButtonCategories[] buttonCategories;

    private LaptopUI laptopUI;
    private ShopManager sm;
    private GameManager gm => GameManager.Instance;
    private ShopManager.OfferCategory currentCategory;

    private void Awake()
    {
        sm = ShopManager.Instance;
        pointsLabelText.text = gm.points.ToString();
        sm.OnPurchaseSceneObject += CloseLaptopUI;
        gm.OnPointsChanged += _ => UpdatePointsLabel();

        foreach (var buttonCategories in buttonCategories)
        {
            buttonCategories.categoryButton.onClick.AddListener(
                () => SetCategory(buttonCategories.offerCategory, buttonCategories.title));
        }
        if (laptopUI == null) laptopUI = FindFirstObjectByType<LaptopUI>();

        currentCategory = ShopManager.OfferCategory.Device;
        ShowOffers(currentCategory);
    }

    private void UpdatePointsLabel()
    {
        pointsLabelText.text = gm.points.ToString();
    }

    private void SetScrollbarVisibility(bool visible)
    {
        scrollbarGroup.gameObject.SetActive(visible);
    }

    /// <summary>
    /// Отображение товаров нужной категории
    /// </summary>
    /// <param name="category"></param>
    private void ShowOffers(ShopManager.OfferCategory category)
    {
        ClearPreviousOffers();
        var neededOffers = ShopManager.Instance.offers[category.ToString()];
        foreach (var offer in neededOffers)
        {
            var createdCard = Instantiate(offerCard, offersContainer);
            createdCard.Initialize(offer);
        }
        SetScrollbarVisibility(neededOffers.Count > 4);
    }

    private void ClearPreviousOffers()
    {
        for (var i = offersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(offersContainer.GetChild(i).gameObject);
        }
    }

    private void SetCategory(ShopManager.OfferCategory category, string title)
    {
        ShowOffers(category);
        categoryTitle.text = title;
        AudioManager.Instance.PlaySound(SoundType.UniversalClick);
    }

    private void CloseLaptopUI()
    {
        laptopUI?.ToggleVisibility(false);
    }

    [Serializable]
    public class ButtonCategories
    {
        public string title;
        public Button categoryButton;
        public ShopManager.OfferCategory offerCategory;
    }
}
