using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LaptopUI : MonoBehaviour
{
    private const string cancelButton = "Cancel";
    [SerializeField] private Button closeButton;
    [SerializeField] private Ease visibilityEase = Ease.OutQuad;
    private CanvasGroup canvasGroup;
    private GameManager gm;
    [SerializeField] private bool hideOnPlay = true;

    private void Awake()
    {
        gm = GameManager.Instance;
        canvasGroup = GetComponent<CanvasGroup>();
        closeButton.onClick.AddListener(() => ToggleVisibility(false));
        if (hideOnPlay)
        {
            canvasGroup.alpha = 0f;
            ToggleVisibility(false);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown(cancelButton)) ToggleVisibility(false);
    }


    public event Action<bool> OnVisibilityChanged;

    public void ToggleVisibility(bool toggle)
    {
        if (toggle)
        {
            gameObject.SetActive(true);
            AudioManager.Instance.PlaySound(SoundType.OpenLaptop, 0.3f);
        }
        canvasGroup.DOKill();

        canvasGroup.DOFade(toggle ? 1f : 0f, 0.3f)
            .SetEase(visibilityEase)
            .OnComplete(() =>
            {
                if (!toggle) gameObject.SetActive(false);
            });

        OnVisibilityChanged?.Invoke(toggle);
        canvasGroup.interactable = toggle;
        canvasGroup.blocksRaycasts = toggle;
    }

    // public void HideLaptopUI()
    // {
    //     canvasGroup.DOKill();
    //     canvasGroup.DOFade(0f, 0.3f)
    //         .SetEase(visibilityEase)
    //         .OnComplete(() =>
    //         {
    //             ToggleVisibility(false);
    //             gameObject.SetActive(false);
    //         });
    // }
}