using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private RectTransform phoneRectTransform;
    [SerializeField] private float uiToCameraScale = 0.01f;

    private Vector3 initialCameraPosition;

    private void Start()
    {
        initialCameraPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector2 phoneDisplacement = phoneRectTransform.anchoredPosition;

        Vector3 newPosition = initialCameraPosition + new Vector3(
            phoneDisplacement.x * uiToCameraScale,
            phoneDisplacement.y * uiToCameraScale,
            0
        );

        transform.position = newPosition;
    }
}