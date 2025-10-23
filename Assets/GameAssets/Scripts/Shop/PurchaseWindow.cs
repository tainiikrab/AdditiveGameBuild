using _Scripts.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseWindow : MonoBehaviour
{
    public TextMeshProUGUI offerName;
    public TextMeshProUGUI offerDescription;
    public TextMeshProUGUI offerPrice;
    public Image offerImage;
    public TextMeshProUGUI playerBalance;

    private int _price;
    private ShopItemConfig offerData;
    private ShopManager sm;

    public void Initialize(ShopItemConfig item)
    {
        offerData = item;
        _price = item.price;

        offerName.text = offerData.name;
        offerDescription.text = offerData.description;
        offerImage.sprite = item.Icon;
        offerImage.preserveAspect = true;

        playerBalance.text = GameManager.Instance.points.ToString();

        InitTextPrice(_price.ToString());
    }

    private void InitTextPrice(string text)
    {
        offerPrice.text = text;
        if (GameManager.Instance.points < _price) offerPrice.color = Color.red;
    }

    public void OnClickCancelButton()
    {
        Destroy(gameObject);
        AudioManager.Instance.PlayClickSound();
    }

    public void OnClickBuyButton()
    {
        if (GameManager.Instance.points < _price) return;
        sm.Purchase(offerData);
        Destroy(gameObject);
        AudioManager.Instance.PlayClickSound();
    }
}