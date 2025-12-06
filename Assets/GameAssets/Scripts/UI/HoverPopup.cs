using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class HoverPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform popup;

    private bool isHoveringElement;
    private bool isHoveringPopup;
    private CanvasGroup popupCanvasGroup;
    private Tween currentTween;

    private void Awake()
    {
        if (popup != null)
        {
            popupCanvasGroup = popup.GetComponent<CanvasGroup>();
            if (popupCanvasGroup == null)
                popupCanvasGroup = popup.gameObject.AddComponent<CanvasGroup>();

            popupCanvasGroup.alpha = 0f;
            popup.localScale = Vector3.one * 0.8f; // старт чуть меньше
            popup.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject == popup.gameObject)
            isHoveringPopup = true;
        else
            isHoveringElement = true;

        ShowPopup();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject == popup.gameObject)
            isHoveringPopup = false;
        else
            isHoveringElement = false;

        TryHidePopup();
    }

    private void ShowPopup()
    {
        if (popup == null) return;

        popup.gameObject.SetActive(true);
        currentTween?.Kill();

        // параллельная анимация прозрачности и масштаба
        var seq = DOTween.Sequence();
        seq.Join(popupCanvasGroup.DOFade(1f, 0.25f));
        seq.Join(popup.DOScale(1f, 0.35f).SetEase(Ease.OutBack, 1.2f));
        // OutBack с небольшой амплитудой (overshoot 1.2 вместо дефолтного 1.7)
        currentTween = seq;
    }

    private void HidePopup()
    {
        if (popup == null) return;

        currentTween?.Kill();

        var seq = DOTween.Sequence();
        seq.Join(popupCanvasGroup.DOFade(0f, 0.2f));
        seq.Join(popup.DOScale(0.8f, 0.2f).SetEase(Ease.InQuad));
        seq.OnComplete(() => popup.gameObject.SetActive(false));
        currentTween = seq;
    }

    private void TryHidePopup()
    {
        if (!isHoveringElement && !isHoveringPopup)
            HidePopup();
    }
}