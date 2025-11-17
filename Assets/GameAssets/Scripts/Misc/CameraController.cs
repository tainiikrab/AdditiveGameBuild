using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private const string panButton = "PanButton";
    private const string zoomAxis = "ZoomAxis";
    private const string verticalAxis = "VerticalAxis";
    private const string horizontalAxis = "HorizontalAxis";
    private const string mouseXAxis = "Mouse X";
    private const string mouseYAxis = "Mouse Y";

    [Header("Speed")] [SerializeField] private float basePanSpeed = 20f;

    [SerializeField] private float baseMousePanSpeed = 1f;

    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float zoomLerpAmount = 1f;

    [Header("Zoom")] [SerializeField] private float minZoom = 2f;

    [SerializeField] private float maxZoom = 20f;

    [Header("Boundaries")] [SerializeField]
    private float xBoundary = 20f;

    [SerializeField] private float yBoundary = 20f;

    private Camera cam;
    private CinemachineCamera vcam;
    private float camTargetSize;
    private GameManager gm;

    [SerializeField] private bool handlePan = true;

    private void Start()
    {
        InitializeCamera();
        gm = GameManager.Instance;
    }

    [SerializeField] private bool blockMovementOverUI = false;


    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    private void Update()
    {
        if (blockMovementOverUI && IsPointerOverUI())
            return;

        HandleZoom();
        if (!handlePan) return;

        HandlePan();
    }

    private bool useVcam = false;

    private void InitializeCamera()
    {
        cam = GetComponent<Camera>();
        vcam = GetComponent<CinemachineCamera>();

        if (vcam != null)
        {
            camTargetSize = vcam.Lens.OrthographicSize;
            useVcam = true;
        }
        else if (cam != null)
        {
            camTargetSize = cam.orthographicSize;
            if (!cam.orthographic)
            {
                Debug.LogWarning("Camera is not orthographic. Enabling orthographic mode.");
                cam.orthographic = true;
            }
        }
        else
        {
            Debug.LogError("No Camera or CinemachineCamera found!");
        }
    }

    private void HandlePan()
    {
        var gotInput = false;
        var panDirection = Vector3.zero;

        float panSpeedMultiplier;
        if (useVcam)
            panSpeedMultiplier = vcam.Lens.OrthographicSize / maxZoom;
        else
            panSpeedMultiplier = cam.orthographicSize / maxZoom;


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
            var h = -Input.GetAxis(mouseXAxis) * panSpeedMultiplier * baseMousePanSpeed;
            var v = -Input.GetAxis(mouseYAxis) * panSpeedMultiplier * baseMousePanSpeed;

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

        var newOrthoSize = Mathf.Lerp(
            GetCurrentOrthographicSize(),
            camTargetSize,
            zoomLerpAmount * Time.unscaledDeltaTime
        );

        ApplyOrthographicSize(newOrthoSize);
    }

    private float GetCurrentOrthographicSize()
    {
        if (useVcam)
            return vcam.Lens.OrthographicSize;
        return cam.orthographicSize;
    }

    private void ApplyOrthographicSize(float size)
    {
        if (useVcam)
            vcam.Lens.OrthographicSize = size;
        else cam.orthographicSize = size;
    }
}