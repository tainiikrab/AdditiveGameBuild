using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePhone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform phoneRect;
    [SerializeField] private RectTransform screenRect;

    private bool isDragging;
    private Vector2 offset;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(screenRect, eventData.position, eventData.pressEventCamera))
        {
            isDragging = false;
        }
        else
        {
            isDragging = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(phoneRect.parent as RectTransform, eventData.position, eventData.pressEventCamera, out offset);
            offset = phoneRect.anchoredPosition - offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(phoneRect.parent as RectTransform, eventData.position, eventData.pressEventCamera, out var localPoint))
        {
            phoneRect.anchoredPosition = localPoint + offset;
        }
    }
}
