using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShopItem : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI offerName;
    [SerializeField] protected Image offerIcon;
    [SerializeField] protected Button buyButton;
    [SerializeField] protected TextMeshProUGUI offerPrice;
    
    protected ShopItemConfig _itemData;
    
    public virtual void Initialize(ShopItemConfig item)
    {
        _itemData = item;
        
        if (_itemData.name != null) offerName.text = _itemData.name;
        if (_itemData.Icon != null)
        {
            offerIcon.sprite = _itemData.Icon;
            offerIcon.preserveAspect = true;
        }
        UpdateItemsState();
        if (buyButton != null) buyButton.onClick.AddListener(() => OnClickBuyButton(_itemData));
        ShopManager.OnItemsStateChanged += UpdateItemsState;
    }
    
    protected void OnClickBuyButton(ShopItemConfig item)
    {
        if (!ShopManager.CanAfford(item)) return;
        if (ShopManager.IsPurchased(item)) return;
        ShopManager.TryPurchase(item);
    }
    
    protected void OnDestroy()
    {
        ShopManager.OnItemsStateChanged -= UpdateItemsState;
    }
    
    protected void UpdateItemsState()
    {
        if (ShopManager.IsPurchased(_itemData))
        {
            buyButton.interactable = false;
            offerPrice.text = "Куплено";
            buyButton.image.color = Color.cyan;
            offerPrice.color = Color.white;
        }
        else if (!ShopManager.CanAfford(_itemData))
        {
            buyButton.interactable = false;
            offerPrice.text = _itemData.price.ToString();
            offerPrice.color = Color.red;
        }
        else
        {
            buyButton.interactable = true;
            offerPrice.text = _itemData.price.ToString();
            offerPrice.color = Color.black;
        }
    }
}