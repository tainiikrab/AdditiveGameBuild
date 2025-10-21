using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private Ease visibilityEase = Ease.OutQuad;
    [SerializeField] private float fadeDuration;
    private Canvas canvas;
    private Button closeButton;
    private RectTransform parent;

    private void Awake()
    {
        parent = transform.parent.GetComponent<RectTransform>();

        canvas = GetCanvas(transform.parent);


        Debug.Log(canvas.name);
        closeButton = GetComponentInChildren<Button>();
        closeButton.onClick.AddListener(() => ToggleVisibility(false));
    }

    void IDragHandler.OnDrag(PointerEventData data)
    {
        parent.anchoredPosition += data.delta / canvas.scaleFactor;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        parent.SetAsLastSibling();
    }

    public static Canvas GetCanvas(Transform parent)
    {
        var testParent = parent;
        while (testParent != null)
        {
            if (testParent.TryGetComponent<Canvas>(out var newCanvas)) return newCanvas;

            testParent = testParent.parent;
        }

        return null;
    }

    private void ToggleVisibility(bool toggle)
    {
        var canvasGroup = parent.GetComponent<CanvasGroup>();
        canvasGroup.DOKill();

        canvasGroup.DOFade(toggle ? 1 : 0, fadeDuration)
            .SetEase(visibilityEase);

        canvasGroup.interactable = toggle;
        canvasGroup.blocksRaycasts = toggle;
    }
}