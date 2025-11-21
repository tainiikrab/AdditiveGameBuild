using UnityEngine;

public class DebugCursorOverlay : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // Get mouse position in screen space
        var mousePos = Input.mousePosition;

        // Convert to world space
        var worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        // "10f" is distance from camera; adjust depending on your setup

        // Move this GameObject to follow the cursor
        transform.position = worldPos;
    }
}