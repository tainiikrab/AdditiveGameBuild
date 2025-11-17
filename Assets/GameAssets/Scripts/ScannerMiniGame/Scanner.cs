using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scanner : MonoBehaviour
{
    [SerializeField] private RectTransform phoneCameraPoint;
    [SerializeField] private float scanTime = 2f;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private PhoneUI phoneUI;
    
    private PointerEventData pointerEventData;
    
    private bool isUIActive;
    private PrintingMaterial currentPrintingMaterial;
    private float scanTimer;

    private void Awake()
    {
        phoneUI.Accepted += OnAccepted;
        phoneUI.Cancelled += OnCancelled;
    }

    private void Update()
    {
        if (!isUIActive)
        {
            Scan();
        }
    }

    private void Scan()
    {
        var data = new PointerEventData(EventSystem.current)
        {
            position = phoneCameraPoint.position
        };
        
        var results = new List<RaycastResult>();
        graphicRaycaster.Raycast(data, results);

        PrintingMaterial detectedHitPrintingMaterial = null;
        
        foreach (var result in results)
        {
            detectedHitPrintingMaterial = result.gameObject.GetComponent<PrintingMaterial>();
            if (detectedHitPrintingMaterial != null)
            {
                break;
            }
        }

        if (detectedHitPrintingMaterial == null)
        {
            scanTimer = 0f;
            currentPrintingMaterial = null;
            return;
        }

        if (currentPrintingMaterial != detectedHitPrintingMaterial)
        {
            currentPrintingMaterial = detectedHitPrintingMaterial;
            scanTimer = 0f;
        }
        
        scanTimer += Time.deltaTime;

        if (scanTimer >= scanTime)
        {
            isUIActive = true;
            phoneUI.Initialize(currentPrintingMaterial);
        }
    }

    private void OnAccepted()
    {
        
        
        isUIActive = false;
        phoneUI.ClosePhoneUI();
    }

    private void OnCancelled()
    {
        currentPrintingMaterial = null;
        scanTimer = 0f;
        isUIActive = false;
        phoneUI.ClosePhoneUI();
    }
}
