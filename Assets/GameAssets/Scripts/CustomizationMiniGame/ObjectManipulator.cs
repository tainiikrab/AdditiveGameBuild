using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{
    public float rotationSpeed = 2.5f;
    public float zoomSpeed = 10f;
    public float minScale = 0.5f;
    public float maxScale = 5f;

    private Vector3 lastMousePosition;
    private bool isRotating = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 delta = currentMousePos - lastMousePosition;

            if (delta.magnitude < 10000f)
            {
                transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.Self);
            }

            lastMousePosition = currentMousePos;
        }

        float scroll = Input.GetAxis("ZoomAxis");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            Vector3 scale = transform.localScale;
            scale += Vector3.one * scroll * zoomSpeed;
            scale = ClampVector3(scale, minScale, maxScale);
            transform.localScale = scale;
        }
    }

    private Vector3 ClampVector3(Vector3 v, float min, float max)
    {
        return new Vector3(
            Mathf.Clamp(v.x, min, max),
            Mathf.Clamp(v.y, min, max),
            Mathf.Clamp(v.z, min, max)
        );
    }
}
