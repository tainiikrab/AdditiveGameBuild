using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine.VFX;

public class Printer : MonoBehaviour, IRaycastInteractable
{
    public GameObject printHead;
    public GameObject printHeadSupport;
    public GameObject display;

    [SerializeField] private Light hoverLight;
    [SerializeField] private CinemachineCamera printerCamera;

    [Header("Print Head Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float moveDuration = 3f;

    [Header("Print Head Support Settings")]
    [SerializeField] private float moveSpeedSupport = 0.5f;
    [SerializeField] private float moveDistanceSupport = 0.1f;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private VisualEffect printEffect;


    private float lightIntensity = 3f;
    private bool isCameraActive = false;
    private bool isPrinting = false;

    private Vector3 startPosition;

    private void Awake()
    {
        if (!hoverLight.gameObject.activeSelf) hoverLight.gameObject.SetActive(true);
        lightIntensity = hoverLight.intensity;
        hoverLight.intensity = 0;

        if (printerCamera != null)
        {
            printerCamera.Priority = -100;
        }

        if (printHead != null)
            startPosition = printHead.transform.localPosition;

        printEffect.Stop();
    }

    private void Update()
    {
        if (isCameraActive && Input.GetKeyDown(KeyCode.P) && !isPrinting)
        {
            StartCoroutine(PrintHeadMoveRoutine());
        }
    }

    public void OnHoverEnter()
    {
        hoverLight.intensity = lightIntensity;
    }

    public void OnHoverExit()
    {
        hoverLight.intensity = 0;
    }

    public void OnClick()
    {
        TogglePrinterCamera();
    }

    public void TogglePrinterCamera()
    {
        if (printerCamera == null) return;

        if (!isCameraActive)
            ActivatePrinterCamera();
        else
            DeactivatePrinterCamera();
    }

    private void ActivatePrinterCamera()
    {
        printerCamera.Priority = 100;
        isCameraActive = true;

        hoverLight.intensity = lightIntensity * 2f;
        Debug.Log("Printer camera activated");
    }

    private void DeactivatePrinterCamera()
    {
        printerCamera.Priority = -100;
        isCameraActive = false;

        hoverLight.intensity = lightIntensity;
        Debug.Log("Printer camera deactivated");
    }

    public void ActivateCameraWithPriority(int priority = 100)
    {
        printerCamera.Priority = priority;
        isCameraActive = true;
    }

    public void DeactivateCamera()
    {
        printerCamera.Priority = -100;
        isCameraActive = false;
    }

    private string FormatTime(float timeSeconds)
    {
        int hours = Mathf.FloorToInt(timeSeconds / 3600f);
        int minutes = Mathf.FloorToInt((timeSeconds % 3600) / 60f);
        int seconds = Mathf.FloorToInt(timeSeconds % 60f);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }


    private IEnumerator PrintHeadMoveRoutine()
    {
        if (printHead == null || printHeadSupport == null) yield break;

        isPrinting = true;

        if (printEffect != null)
        {
            printEffect.gameObject.SetActive(true);
            printEffect.Play();
        }

        Vector3 startPoint = printHead.transform.localPosition;
        Vector3 endPoint = startPoint + new Vector3(moveDistance, 0, 0);
        Vector3 target = endPoint;

        Vector3 startPointSupport = printHeadSupport.transform.localPosition;
        Vector3 endPointSupport = startPointSupport + new Vector3(0, 0, moveDistanceSupport);
        Vector3 targetSupport = endPointSupport;

        float elapsedHead = 0f;

        while (elapsedHead < moveDuration)
        {
            float moveStepHead = moveSpeed * Time.deltaTime;
            float moveStepSupport = moveSpeedSupport * Time.deltaTime;

            printHead.transform.localPosition =
                Vector3.MoveTowards(printHead.transform.localPosition, target, moveStepHead);

            if (Vector3.Distance(printHead.transform.localPosition, target) < 0.001f)
            {
                target = target == endPoint ? startPoint : endPoint;
            }   

            printHeadSupport.transform.localPosition =
                Vector3.MoveTowards(printHeadSupport.transform.localPosition, targetSupport, moveStepSupport);

            if (Vector3.Distance(printHeadSupport.transform.localPosition, targetSupport) < 0.001f)
            {
                targetSupport = targetSupport == endPointSupport ? startPointSupport : endPointSupport;
            }

            elapsedHead += Time.deltaTime;

            float remainingTime = Mathf.Max(moveDuration - elapsedHead, 0f);
            timerText.text = FormatTime(remainingTime);

            yield return null;
        }

        printHead.transform.localPosition = startPoint;
        printHeadSupport.transform.localPosition = startPointSupport;

        timerText.text = "00:00:00";

        printEffect.Stop();
        printEffect.gameObject.SetActive(false);
        

        isPrinting = false;
    }


}
