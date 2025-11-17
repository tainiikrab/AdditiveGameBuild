using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrintingMaterial : MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string title;

    public Sprite Icon => icon;

    public string Title => title;
}
