using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class QRScanner : MonoBehaviour
{
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private Camera scannerCamera;
    [SerializeField] private LayerMask boxLayerMask;
    [SerializeField] private float minLogInterval = 0.2f;

    private Transform camTransform;
    private Box lastLoggedBox;
    private float lastLogTime = -Mathf.Infinity;

    private void Awake()
    {
        if (scannerCamera == null) scannerCamera = GetComponent<Camera>();
        if (scannerCamera == null) scannerCamera = Camera.main;
        camTransform = scannerCamera != null ? scannerCamera.transform : null;
    }

    private void Update()
    {
        if (camTransform == null) return;

        if (Physics.Raycast(camTransform.position, camTransform.forward, out var hit, rayDistance, boxLayerMask))
        {
            var box = hit.collider.GetComponent<Box>();
            if (box != null)
            {
                if (box == lastLoggedBox && !(Time.time - lastLogTime >= minLogInterval)) return;

                Debug.Log($"Коробка номер {box.Number}");
                lastLoggedBox = box;
                lastLogTime = Time.time;
                return;
            }
        }

        lastLoggedBox = null;
    }

    public void ScanOnce()
    {
        if (camTransform == null)
        {
            if (scannerCamera == null) scannerCamera = GetComponent<Camera>();
            if (scannerCamera == null) scannerCamera = Camera.main;
            camTransform = scannerCamera != null ? scannerCamera.transform : null;
        }
        if (camTransform == null) return;

        if (!Physics.Raycast(camTransform.position, camTransform.forward, out var hit, rayDistance, boxLayerMask)) return;
        var box = hit.collider.GetComponent<Box>();
        if (box != null)
        {
            Debug.Log($"Коробка номер {box.Number}");
        }
    }
}
