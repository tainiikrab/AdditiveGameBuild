using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePhone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform phoneRect;
    [SerializeField] private RectTransform screenRect;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float uiToCameraScale = 0.01f; // Константа масштабирования (настраивается в Inspector)

    private bool isDragging;
    private Vector2 offset;
    private Vector3 cameraInitialPosition;

    private void Start()
    {
        // Сохраняем начальную позицию камеры
        cameraInitialPosition = targetCamera.transform.position;
    }

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
            // Обновляем позицию телефона в UI
            phoneRect.anchoredPosition = localPoint + offset;

            // Применяем смещение телефона к камере с масштабированием
            Vector2 phoneDisplacement = phoneRect.anchoredPosition;
            Vector3 newCameraPosition = cameraInitialPosition + new Vector3(
                phoneDisplacement.x * uiToCameraScale,
                phoneDisplacement.y * uiToCameraScale,
                0
            );

            targetCamera.transform.position = newCameraPosition;
        }
    }
}
