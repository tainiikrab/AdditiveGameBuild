using Rellac.Windows;
using UnityEngine;
using UnityEngine.UI;

public class OpenWindowButton : MonoBehaviour
{
    [SerializeField] private GUIWindow windowAsset;
    private Button button;
    private Transform laptopParent;
    public bool isWindowOpen { get; set; }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenWindow);
        laptopParent = transform.parent.parent.parent;
    }

    public void OpenWindow()
    {
        if (isWindowOpen || windowAsset == null) return;
        isWindowOpen = true;

        var window = Instantiate(windowAsset, laptopParent);
        window.Initialize(this);
        // Make sure it's centered
        // var rect = window.GetComponent<RectTransform>();
        //
        // var center = new Vector2(0.5f, 0.5f);
        // rect.anchorMin = center;
        // rect.anchorMax = center;
        // rect.pivot = center;
        // rect.anchoredPosition = Vector2.zero;
    }
}