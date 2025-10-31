using DG.Tweening;
using UnityEngine;

public abstract class UIElement : MonoBehaviour
{
    private CanvasGroup cachedCanvasGroup;
    [SerializeField] protected Ease visibilityEase = Ease.OutQuad;

    protected CanvasGroup canvasGroup
    {
        get
        {
            if (cachedCanvasGroup == null) cachedCanvasGroup = GetComponent<CanvasGroup>();
            return cachedCanvasGroup;
        }
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
}