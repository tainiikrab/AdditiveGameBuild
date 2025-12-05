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

    private ShopUI shopUI;

    /// <summary>
    /// Поле для хранения информации о товаре в данной карточке
    /// </summary>
    private ShopItemConfig thisOffer;

    private void Awake()
    {
        offerPriceText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        shopUI = GetComponentInParent<ShopUI>();
    }

    private void OnEnable()
    {
        ShopManager.Instance.OnPurchase += SetButtonsState;
    }
    /// <summary>
    /// Инициализирует данные о товаре на карточке
    /// </summary>
    /// <param name="offer">Данные о товаре</param>
    public void Initialize(ShopItemConfig offer)
    {
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(375f, 475f);
        
        thisOffer = offer;
        offerIcon.sprite = offer.Icon;
        offerIcon.preserveAspect = true;
        offerName.text = offer.name;
        offerPriceText.text = offer.price.ToString();
        
        buyButton.onClick.RemoveAllListeners();
        moreDetailedButton.onClick.RemoveAllListeners();
        
        buyButton.onClick.AddListener(() => OnBuyButtonClick(thisOffer));
        moreDetailedButton.onClick.AddListener(() => OnMoreDetailedButtonClick(thisOffer));
        
        SetButtonsState();
    }

    private void SetButtonsState()
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy) 
            return;
    
        if (thisOffer == null) return;
    
        if (GameManager.Instance.points < thisOffer.price)
        {
            buyButton.interactable = false;
            buyButton.image.color = Color.brown;
        }
        else if (SaveManager.gameData.purchasedOffers.Contains(thisOffer.id))
        {
            buyButton.interactable = false;
            buyButton.image.color = Color.darkOliveGreen;
            buyButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Куплено";
            moreDetailedButton.interactable = false;
        }
        else
        {
            buyButton.interactable = true;
            buyButton.image.color = Color.white;
            moreDetailedButton.interactable = true;
            moreDetailedButton.image.color = Color.white;
        }
    }

    private void OnBuyButtonClick(ShopItemConfig item)
    {
        Debug.Log("Buying " + item.name);
        ShopManager.Instance.Purchase(item);
    }

    private void OnMoreDetailedButtonClick(ShopItemConfig item)
    {
        var panel = Instantiate(moreDetailedUI, shopUI.gameObject.transform);
        panel.Initialize(item);
    }

    private void OnDestroy()
    {
        ShopManager.Instance.OnPurchase -= SetButtonsState;
        buyButton.onClick.RemoveListener(() => OnBuyButtonClick(thisOffer));
        moreDetailedButton.onClick.RemoveListener(() => OnMoreDetailedButtonClick(thisOffer));
    }
}