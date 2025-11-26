using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferCardUI : MonoBehaviour
{
    [SerializeField] private Image offerIcon;
    [SerializeField] private TextMeshProUGUI offerName;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button moreDetailedButton;
    [SerializeField] private MoreDetailedUI moreDetailedUI;

    private TextMeshProUGUI offerPriceText;

    /// <summary>
    /// Поле для хранения информации о товаре в данной карточке
    /// </summary>
    private ShopItemConfig thisOffer;

    private void Awake()
    {
        offerPriceText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        ShopManager.Instance.OnPurchase += () => SetButtonsState(buyButton, moreDetailedButton);
    }
    /// <summary>
    /// Инициализирует данные о товаре на карточке
    /// </summary>
    /// <param name="offer">Данные о товаре</param>
    public void Initialize(ShopItemConfig offer)
    {
        thisOffer = offer;
        offerIcon.sprite = offer.Icon;
        offerName.text = offer.name;
        offerPriceText.text = offer.price.ToString();
        buyButton.onClick.AddListener(() => OnBuyButtonClick(thisOffer));
        moreDetailedButton.onClick.AddListener(() => OnMoreDetailedButtonClick(thisOffer));
        SetButtonsState(buyButton, moreDetailedButton);
    }

    private void SetButtonsState(Button buyButton, Button detailedButton)
    {
        if (GameManager.Instance.points < thisOffer.price)
        {
            buyButton.interactable = false;
            buyButton.image.color = Color.brown;
        }
        else if (SaveManager.gameData.purchasedOffers.Contains(thisOffer.id))
        {
            buyButton.interactable = false;
            buyButton.image.color = Color.darkOliveGreen;
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Куплено";
            detailedButton.interactable = false;
        }
        else
        {
            buyButton.interactable = true;
            buyButton.image.color = Color.white;
            detailedButton.interactable = true;
            detailedButton.image.color = Color.white;
        }
    }

    private void OnBuyButtonClick(ShopItemConfig item)
    {
        Debug.Log("Buying " + item.name);
        ShopManager.Instance.Purchase(item);
    }

    private void OnMoreDetailedButtonClick(ShopItemConfig item)
    {
        //var panel = Instantiate(moreDetailedUI);
        // потом доделаю)
    }

    private void OnDisable()
    {
        ShopManager.Instance.OnPurchase -= () => SetButtonsState(buyButton, moreDetailedButton);
    }
}