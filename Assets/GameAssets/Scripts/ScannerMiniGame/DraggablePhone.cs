using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePhone : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform phoneRect;
    [SerializeField] private Canvas canvas;

    private bool isDragging;
    private Vector2 offset;
    private RectTransform parentRect;

    private void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        
        parentRect = phoneRect.parent as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, 
            eventData.position, 
            eventData.pressEventCamera, 
            out offset
        );
        offset = phoneRect.anchoredPosition - offset;
        
        phoneRect.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, 
            eventData.position, 
            eventData.pressEventCamera, 
            out var localPoint))
        {
            var newPosition = localPoint + offset;
            
            newPosition = ClampToParentBounds(newPosition);
            
            phoneRect.anchoredPosition = newPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    private Vector2 ClampToParentBounds(Vector2 position)
    {
        if (parentRect == null) return position;

        var parentBounds = parentRect.rect;
        var phoneBounds = phoneRect.rect;

        var minX = parentBounds.xMin - phoneBounds.xMin;
        var maxX = parentBounds.xMax - phoneBounds.xMax;
        var minY = parentBounds.yMin - phoneBounds.yMin;
        var maxY = parentBounds.yMax - phoneBounds.yMax;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}