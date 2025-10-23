using _Scripts.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardUI : MonoBehaviour
{
    public GameObject infoWindowPrefab;

    [Space(5)] public Image offerIcon;

    public TextMeshProUGUI offerName;
    public TextMeshProUGUI offerPrice;
    public Button buyButton;
    public Button questionButton;

    private int _price;
    private ShopUI _shopUI;
    private ShopItemConfig offerData;

    private GameManager gm;
    private ShopManager sm;

    private void Awake()
    {
        _shopUI = GetComponentInParent<ShopUI>();
    }

    private void Start()
    {
        Debug.Log($"ShopCardUI Start: {GameManager.Instance}");
        gm = GameManager.Instance;
        sm = ShopManager.Instance;
    }

    private void Update()
    {
        InitTextPrice(_price.ToString());
    }

    public void Initialize(ShopItemConfig item)
    {
        gm = GameManager.Instance;
        offerData = item;
        _price = offerData.price;

        if (offerData.Icon != null)
        {
            offerIcon.sprite = offerData.Icon;
            offerIcon.preserveAspect = true;
        }

        if (offerData.name != null) offerName.text = offerData.name;

        InitTextPrice(_price.ToString());
    }

    private void InitTextPrice(string text)
    {
        offerPrice.text = text;

        // Check if InventoryManager is initialized
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager not initialized yet!");
            return;
        }

        Debug.Log(gm.points);
        Debug.Log(InventoryManager.Instance);
        Debug.Log(offerData);
        Debug.Log(InventoryManager.Instance.HasItem(offerData));
        if (InventoryManager.Instance.HasItem(offerData))
        {
            questionButton.interactable = false;
            questionButton.onClick.RemoveAllListeners();
            questionButton.image.color = Color.aquamarine;

            OnPurchased();
        }

        else if (gm.points < _price && !InventoryManager.Instance.HasItem(offerData))
        {
            offerPrice.color = Color.red;
        }
    }

    public void OnClickBuyButton()
    {
        if (gm.points < _price) return;
        sm.Purchase(offerData);

        questionButton.interactable = false;
        questionButton.image.color = Color.aquamarine;
        questionButton.onClick.RemoveAllListeners();
        AudioManager.Instance.PlayClickSound();
    }

    private void OnPurchased()
    {
        buyButton.interactable = false;
        buyButton.image.color = Color.aquamarine;
        buyButton.onClick.RemoveAllListeners();
        offerPrice.text = "Куплено";
    }

    public void OnClickQuestion()
    {
        var infoWindow = Instantiate(infoWindowPrefab, _shopUI.transform);
        infoWindow.GetComponent<PurchaseWindow>().Initialize(offerData);
        AudioManager.Instance.PlayClickSound();
    }
}