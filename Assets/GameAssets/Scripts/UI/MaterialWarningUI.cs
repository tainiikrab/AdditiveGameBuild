using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialWarningUI : MonoBehaviour
{
    [SerializeField] private Button replenishButton;

    private void Awake()
    {
        replenishButton.onClick.AddListener(OnReplenishClick);
    }

    private void OnReplenishClick()
    {
        MaterialManager.Instance.Replenish();
        //GameManager.Instance.points -= 1000;
        gameObject.SetActive(false);
    }
}