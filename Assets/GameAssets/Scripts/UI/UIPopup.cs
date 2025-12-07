using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private GameObject body;

    public void Awake()
    {
        body.gameObject.SetActive(false);
    }
    
    public void Show()
    {
        body.SetActive(true);
        AudioManager.Instance.PlaySound(SoundType.UniversalClick);
    }

    public void Hide()
    {
        body.SetActive(false);
        AudioManager.Instance.PlaySound(SoundType.Cancel);
    }
}