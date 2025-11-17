using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUI : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialTitle;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;
    
    public event Action Accepted;
    public event Action Cancelled;
    
    public void Initialize(PrintingMaterial material)
    {
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
    }
}
