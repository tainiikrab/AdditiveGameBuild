using _Scripts.Shop;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIButton : MonoBehaviour
{
    [SerializeField] private ShopManager.Category category;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        ShopManager.Instance.SetCategory(category);
    }
}