using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const string panButton = "PanButton";
    private const string zoomAxis = "ZoomAxis";
    private const string verticalAxis = "VerticalAxis";
    private const string horizontalAxis = "HorizontalAxis";
    private const string mouseXAxis = "Mouse X";
    private const string mouseYAxis = "Mouse Y";

    [Header("Speed")] [SerializeField] private float basePanSpeed = 20f;

    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float zoomLerpAmount = 1f;

    [Header("Zoom")] [SerializeField] private float minZoom = 2f;

    [SerializeField] private float maxZoom = 20f;

    [Header("Boundaries")] [SerializeField]
    private float xBoundary = 20f;

    [SerializeField] private float yBoundary = 20f;

    private Camera cam;
    private float camTargetSize;
    private GameManager gm;

    private void Start()
    {
        InitializeCamera();
        gm = GameManager.Instance;
    }

    private bool isMovementAvailable = true;

    private void Update()
    {
        if (!isMovementAvailable) return;
        HandlePan();
        HandleZoom();
    }

    private void InitializeCamera()
    {
        cam = Camera.main;
        camTargetSize = cam.orthographicSize;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
        }
        else if (!cam.orthographic)
        {
            Debug.LogWarning("Camera is not orthographic. Switching it to orthographic mode.");
            cam.orthographic = true;
        }
    }

    private void HandlePan()
    {
        var gotInput = false;
        var panDirection = Vector3.zero;

        var panSpeedMultiplier = cam.orthographicSize / maxZoom;

        if (TryGetAxisValue(verticalAxis, out var verticalValue))
        {
            panDirection += basePanSpeed * panSpeedMultiplier * Time.deltaTime * new Vector3(0, verticalValue, 0);
            gotInput = true;
        }

        if (TryGetAxisValue(horizontalAxis, out var horizontalValue))
        {
            panDirection += basePanSpeed * panSpeedMultiplier * Time.deltaTime * new Vector3(horizontalValue, 0, 0);
            gotInput = true;
        }
        // if (gotInput)
        //     panDirection.Normalize();

        if (Input.GetButton(panButton)) //middle mouse button
        {
            var h = -Input.GetAxis(mouseXAxis) * panSpeedMultiplier;
            var v = -Input.GetAxis(mouseYAxis) * panSpeedMultiplier;

            gotInput = true;

            panDirection += new Vector3(h, v, 0);
        }

        if (!gotInput) return;

        transform.Translate(panDirection);
        transform.localPosition = ClampPosition(transform.localPosition);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, -xBoundary, xBoundary),
            Mathf.Clamp(position.y, -yBoundary, yBoundary),
            position.z
        );
    }

    private bool TryGetAxisValue(string axisName, out float value)
    {
        value = Input.GetAxis(axisName);
        return Math.Abs(value) >= 0.01f;
    }


    private void HandleZoom()
    {
        if (TryGetAxisValue(zoomAxis, out var zoomValue))
        {
            camTargetSize -= zoomValue * zoomSpeed;
            camTargetSize = Mathf.Clamp(camTargetSize, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, camTargetSize, zoomLerpAmount * Time.unscaledDeltaTime);
    }
}