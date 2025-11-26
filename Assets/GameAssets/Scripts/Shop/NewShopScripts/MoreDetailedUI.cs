using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoreDetailedUI : MonoBehaviour
{
    [SerializeField] private Image offerIcon;
    [SerializeField] private TextMeshProUGUI offerName;
    [SerializeField] private TextMeshProUGUI offerDescription;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button cancelButton;
    
    private TextMeshProUGUI offerPriceText;

    private void Awake()
    {
        offerPriceText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Initialize(ShopItemConfig offer)
    {
        offerIcon.sprite = offer.Icon;
        offerName.text = offer.name;
        offerPriceText.text = offer.price.ToString();
        offerDescription.text = offer.description;
        buyButton.onClick.AddListener(OnBuyButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
    }
    
    private void OnBuyButtonClick()
    {
        
    }
    
    private void OnCancelButtonClick()
    {
        Destroy(gameObject);
    }

    private void UpdateButtonState()
    {
        
    }
}