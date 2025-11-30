using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialProperty : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI propertyNameText;
    [SerializeField] private Image image;

    public void InitializeProperty(string text, Color textColor, Color imageColor)
    {
        propertyNameText.text = text;
        propertyNameText.color = textColor;
        image.color = imageColor;
    }
}
