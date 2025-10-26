using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseWindow : ShopItem
{
    [SerializeField] private TextMeshProUGUI offerDescription;
    [SerializeField] private Button cancelButton;
    
    public override void Initialize(ShopItemConfig item)
    {
        base.Initialize(item);
        
        offerDescription.text = _itemData.description;
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }
    
    private void OnClickCancelButton()
    {
        Destroy(gameObject);
    }
}