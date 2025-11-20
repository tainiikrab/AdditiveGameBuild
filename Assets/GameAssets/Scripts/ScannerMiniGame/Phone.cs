using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Phone : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private RectTransform phonePositionWithUI;
    [SerializeField] private RectTransform phoneStartPosition;
    [SerializeField] private Image shading;
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialTitle;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;
    
    private DraggablePhone draggablePhone;
    
    public event Action Accepted;
    public event Action Cancelled;

    private void Start()
    {
        draggablePhone = GetComponent<DraggablePhone>();
        transform.position = phoneStartPosition.position;
    }
    
    public void Initialize(PrintingMaterial material)
    {
        draggablePhone.enabled = false;
        shading.gameObject.SetActive(true);
        transform.position = phonePositionWithUI.position;
        
        acceptButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        
        screen.SetActive(true);

        materialIcon.sprite = material.Icon;
        materialTitle.text = material.Title;
        
        acceptButton.onClick.AddListener(() => Accepted?.Invoke());
        cancelButton.onClick.AddListener(() => Cancelled?.Invoke());
    }
    
    public void ClosePhoneUI()
    {
        screen.SetActive(false);
        shading.gameObject.SetActive(false);
        transform.position = phoneStartPosition.position;
        draggablePhone.enabled = true;
    }
}
