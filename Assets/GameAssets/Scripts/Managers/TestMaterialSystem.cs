using UnityEngine;
using UnityEngine.UI;

public class TestMaterialSystem : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(Click);
    }
    
    private void Click()
    {
        MaterialManager.Instance.SubtractCount(8);
    }
}
