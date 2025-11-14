using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUI : MonoBehaviour
{
    [SerializeField] private GameObject screen;
    
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialTitle;
    [SerializeField] private TextMeshProUGUI materialDescription;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;
    
    private void Initialize(ShopItemConfig material)
    {
        screen.SetActive(true);

        materialIcon.sprite = material.Icon;
        materialTitle.text = material.name;
        materialDescription.text = material.description;
        
        acceptButton.onClick.AddListener(OnAccept);
        cancelButton.onClick.AddListener(OnCancel);
    }
    
    private void OnAccept()
    {
        // сохраняем материал в менеджер, переносим его в главную сцену
    }

    private void OnCancel()
    {
        screen.SetActive(false);
    }
}
