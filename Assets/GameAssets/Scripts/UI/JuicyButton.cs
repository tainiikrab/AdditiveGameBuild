using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class JuicyUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Targets")] [SerializeField] private RectTransform target;
    [SerializeField] private Graphic targetGraphic;
    [SerializeField] private Button button;

    [Header("Hover")] [SerializeField] [Range(1f, 1.2f)]
    private float hoverScale = 1.06f;

    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutQuad;

    [Header("Press")] [SerializeField] [Range(0.8f, 1f)]
    private float pressScale = 0.95f;

    [SerializeField] private float pressDuration = 0.08f;
    [SerializeField] private Ease pressEase = Ease.OutQuad;

    private Vector3 _baseScale;
    private Tweener _scaleTween;
    private bool _isHovering;
    private bool _isPressed;

    private void Reset()
    {
        target = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        targetGraphic = GetComponent<Graphic>();
    }

    private void Awake()
    {
        if (!target) target = GetComponent<RectTransform>();
        if (!button) button = GetComponent<Button>();
        if (!targetGraphic) targetGraphic = GetComponent<Graphic>();

        _baseScale = target.localScale;
    }

    private void KillScale()
    {
        if (_scaleTween != null && _scaleTween.IsActive()) _scaleTween.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        _isHovering = true;
        AnimateHoverIn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        _isHovering = false;
        if (!_isPressed) AnimateHoverOut();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        _isPressed = true;
        AnimatePress();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        _isPressed = false;
        AnimateRelease();
    }

    private bool IsInteractable()
    {
        return button == null || button.interactable;
    }

    private void AnimateHoverIn()
    {
        KillScale();
        _scaleTween = target.DOScale(_baseScale * hoverScale, hoverDuration)
            .SetEase(hoverEase)
            .SetUpdate(true);
    }

    private void AnimateHoverOut()
    {
        KillScale();
        _scaleTween = target.DOScale(_baseScale, hoverDuration)
            .SetEase(hoverEase)
            .SetUpdate(true);
    }

    private void AnimatePress()
    {
        KillScale();
        _scaleTween = target.DOScale(_baseScale * pressScale, pressDuration)
            .SetEase(pressEase)
            .SetUpdate(true);
    }

    private void AnimateRelease()
    {
        KillScale();
        var targetScale = _isHovering ? _baseScale * hoverScale : _baseScale;
        _scaleTween = target.DOScale(targetScale, hoverDuration)
            .SetEase(hoverEase)
            .SetUpdate(true);
    }
}