using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Phone : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private RectTransform phonePositionWithUI;
    [SerializeField] private RectTransform phoneStartPosition;
    [SerializeField] private Image shading;
    
    [Space(10), Header("Material UI elements")]
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialTitle;
    [SerializeField] private TextMeshProUGUI materialDescription;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private MaterialProperty materialProperty;
    [SerializeField] private RectTransform materialPropertiesContainer;
    
    private DraggablePhone draggablePhone;
    private RectTransform rectTransform;
    
    [Space(10), Header("Animation Settings")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float bigScale = 1.5f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float shadingAlpha = 0.065f;
    
    public event Action Accepted;
    public event Action Cancelled;

    private void Start()
    {
        draggablePhone = GetComponent<DraggablePhone>();
        rectTransform = GetComponent<RectTransform>();
        transform.position = phoneStartPosition.position;
    }
    
    public void Initialize(PrintingMaterialConfig material)
    {
        draggablePhone.enabled = false;
        shading.gameObject.SetActive(true);
        
        acceptButton.interactable = cancelButton.interactable = false;
        
        acceptButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        
        screen.SetActive(true);

        materialIcon.sprite = material.Icon;
        materialTitle.text = material.name;
        materialDescription.text = material.description;
        
        acceptButton.onClick.AddListener(() => Accepted?.Invoke());
        cancelButton.onClick.AddListener(() => Cancelled?.Invoke());

        ClearOldProperties(materialPropertiesContainer);

        foreach (var property in material.GoodProperties)
        {
            Instantiate(materialProperty, materialPropertiesContainer);
            materialProperty.InitializeProperty(property, 
                new Color(0f, 131f/255f, 4f/255f), 
                new Color(140f/255f, 255f/255f, 0f));
        }

        foreach (var property in material.BadProperties)
        {
            Instantiate(materialProperty, materialPropertiesContainer);
            materialProperty.InitializeProperty(property, 
                new Color(131f/255f, 0f, 0f), 
                new Color(1f, 0f, 0f));
        }

        OnScannedAnimation();

        LayoutRebuilder.ForceRebuildLayoutImmediate(materialPropertiesContainer);
    }
    
    public void ClosePhoneUI()
    {
        ContinueScanAnimation();
        screen.SetActive(false);
    }

    private void ClearOldProperties(RectTransform container)
    {
        if (container.childCount == 0) return;     
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    private void OnScannedAnimation()
    {
                var sequence = DOTween.Sequence();
        sequence.Join(
            rectTransform.DOMove(phonePositionWithUI.position, duration).SetEase(Ease.InCubic)
            );
        sequence.Join(
            rectTransform.DOScale(Vector3.one * bigScale, duration).SetEase(Ease.InCubic)
            );
        sequence.Join(
            shading.DOFade(shadingAlpha, duration)
        );

        sequence.OnComplete(() =>
        {
            acceptButton.interactable = cancelButton.interactable = true;
        });
    }

    private void ContinueScanAnimation()
    {
                var sequence = DOTween.Sequence();
        sequence.Join(
            rectTransform.DOScale(Vector3.one * normalScale, duration).SetEase(Ease.InCubic)
            );
        sequence.Join(
            rectTransform.DOMove(phoneStartPosition.position, duration).SetEase(Ease.InCubic)
            );
        sequence.Join(
            shading.DOFade(0f, duration)
            );
        sequence.OnComplete(() =>
        {
            shading.gameObject.SetActive(false);
            draggablePhone.enabled = true;
        });
    }
}
