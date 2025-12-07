using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MoreDetailedUI : MonoBehaviour
{
    [SerializeField] private Image offerIcon;
    [SerializeField] private TextMeshProUGUI offerName;
    [SerializeField] private TextMeshProUGUI offerDescription;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Image shading;

    private TextMeshProUGUI offerPriceText;

    private ShopItemConfig thisOffer;

    private void Awake()
    {
        offerPriceText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialize(ShopItemConfig offer)
    {
        AppearAnimation();
        thisOffer = offer;
        offerIcon.sprite = offer.Icon;
        offerName.text = offer.name;
        offerPriceText.text = offer.price.ToString();
        offerDescription.text = offer.description;
        buyButton.onClick.AddListener(() => OnBuyButtonClick(thisOffer));
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        SetButtonState(buyButton);
    }

    private void OnBuyButtonClick(ShopItemConfig item)
    {
        CloseAnimation();
        ShopManager.Instance.Purchase(item);
        AudioManager.Instance.PlaySound(SoundType.UniversalClick);
    }

    private void AppearAnimation()
    {
        shading.color = new Color(0, 0, 0, 0);
        shading.DOFade(0.5f, 0.2f).SetEase(Ease.OutQuad);
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }

    private void OnCancelButtonClick()
    {
        CloseAnimation();
    }

    private void SetButtonState(Button buyButton)
    {
        if (GameManager.Instance.points < thisOffer.price)
        {
            buyButton.interactable = false;
            buyButton.image.color = Color.brown;
        }
    }

private void CloseAnimation()
    {
        gameObject.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete
        (
            () =>
            {
                if (shading != null)
                {
                    shading.DOFade(0f, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
                        {
                            if (gameObject != null) Destroy(gameObject);
                        }
                    );
                }
                else
                {
                    if (gameObject != null) Destroy(gameObject);
                }
            }
        );
    }
}