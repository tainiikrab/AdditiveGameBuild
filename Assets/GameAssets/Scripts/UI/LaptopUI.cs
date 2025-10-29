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


    private void Awake()
    {
        gm = GameManager.Instance;
        canvasGroup = GetComponent<CanvasGroup>();
        closeButton.onClick.AddListener(() => ToggleVisibility(false));
        canvasGroup.alpha = 0f;
        ToggleVisibility(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown(cancelButton)) ToggleVisibility(false);
    }


    public void ToggleVisibility(bool toggle)
    {
        canvasGroup.DOKill();

        canvasGroup.DOFade(toggle ? 1f : 0f, 0.3f)
            .SetEase(visibilityEase)
            .OnComplete(() =>
            {
                if (!toggle) gameObject.SetActive(false);
            });

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