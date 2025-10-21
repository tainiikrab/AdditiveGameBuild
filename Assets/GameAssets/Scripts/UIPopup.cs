using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] private GameObject hintBody;

    public void Show()
    {
        hintBody.SetActive(true);
        AudioManager.Instance.PlayClickSound();
    }

    public void Hide()
    {
        hintBody.SetActive(false);
        AudioManager.Instance.PlayClickSound();
    }
}