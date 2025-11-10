using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private RectTransform phoneRectTransform;
    [SerializeField] private float uiToCameraScale = 0.01f; // Константа масштабирования

    private Vector3 initialCameraPosition;

    private void Start()
    {
        // Сохраняем начальную позицию камеры
        initialCameraPosition = transform.position;
    }

    private void LateUpdate()
    {
        // Получаем смещение телефона в Canvas
        Vector2 phoneDisplacement = phoneRectTransform.anchoredPosition;

        // Применяем смещение к камере с масштабированием
        Vector3 newPosition = initialCameraPosition + new Vector3(
            phoneDisplacement.x * uiToCameraScale,
            phoneDisplacement.y * uiToCameraScale,
            0
        );

        transform.position = newPosition;
    }
}
