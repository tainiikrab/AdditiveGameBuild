using UnityEngine;
using UnityEngine.UI;
using TMPro;

class ScreenUI : MonoBehaviour
{
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialTitle;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button cancelButton;

    private void Initialize(PrintingMaterialConfig material)
    {
        //materialIcon.sprite = material.Icon;
        materialTitle.text = material.name;
    }
}