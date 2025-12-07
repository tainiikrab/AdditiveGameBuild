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
    public GameObject modelFloor;
    public GameObject defaultModel;
    public Material dissMaterial;

    [SerializeField] private Light hoverLight;
    [SerializeField] private CinemachineCamera printerCamera;

    [Header("Print Head Settings")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float moveDuration = 3f;

    [SerializeField] private float epsilon = 3f;


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


    public IEnumerator PrintHeadMoveRoutine()
    {
        if (printHead == null || printHeadSupport == null) yield break;

        GameObject spawnedObj = null;
        Renderer rend = null;
        Material matInstance = null;
        float minHeight = 0f;
        float maxHeight = 0f;

        if (defaultModel != null && modelFloor != null)
        {
            Vector3 spawnPosition = modelFloor.transform.position;
            spawnedObj = Instantiate(defaultModel, spawnPosition, defaultModel.transform.rotation);

            spawnedObj.transform.localScale = Vector3.one * 0.5f;

            rend = spawnedObj.GetComponent<Renderer>();

            if (rend == null)
            {
                rend = spawnedObj.AddComponent<MeshRenderer>();

                if (spawnedObj.GetComponent<MeshFilter>() == null)
                {
                    spawnedObj.AddComponent<MeshFilter>();
                }
            }

            matInstance = new Material(dissMaterial);
            rend.material = matInstance;
            Bounds bounds = rend.bounds;
            minHeight = bounds.min.y;
            maxHeight = bounds.max.y;

            matInstance.SetFloat("_MinHeight", minHeight);
            matInstance.SetFloat("_MaxHeight", maxHeight);
            matInstance.SetFloat("_DissolveAmount", 1f);

            minHeight = bounds.min.z;
            maxHeight = bounds.max.z;

            float modelMinX_world = bounds.min.x;
            float modelMaxX_world = bounds.max.x;

            Transform root = this.transform;

            float modelMinX_local = root.InverseTransformPoint(new Vector3(modelMinX_world, 0, 0)).x;
            float modelMaxX_local = root.InverseTransformPoint(new Vector3(modelMaxX_world, 0, 0)).x;

            headStartX = modelMinX_local;
            headEndX   = modelMaxX_local;
        }

        isPrinting = true;

        Vector3 startPoint = printHead.transform.localPosition;
        Vector3 endPoint = startPoint + new Vector3(moveDistance, 0, 0);
        startPoint.x = headStartX;
        endPoint.x = headEndX;
        Vector3 target = endPoint;

        Vector3 startPointSupport = printHeadSupport.transform.localPosition;

        float elapsedHead = 0f;
        float objectHeight = maxHeight - minHeight;

        while (elapsedHead < moveDuration)
        {
            float moveStepHead = moveSpeed * Time.deltaTime;

            printHead.transform.localPosition =
                Vector3.MoveTowards(printHead.transform.localPosition, target, moveStepHead);

            if (Vector3.Distance(printHead.transform.localPosition, target) < 0.001f)
            {
                target = target == endPoint ? startPoint : endPoint;
            }

            elapsedHead += Time.deltaTime;

            if (matInstance != null)
            {
                float t = Mathf.Clamp01(elapsedHead / moveDuration);

                float dissolveValue = Mathf.Lerp(1f, 0f, t);
                matInstance.SetFloat("_DissolveAmount", dissolveValue);

                float Growth = epsilon * epsilon * t;

                Vector3 supportPos = printHeadSupport.transform.localPosition;
                supportPos.z = startPointSupport.z + objectHeight * Growth;
                printHeadSupport.transform.localPosition = supportPos;
            }

            float remainingTime = Mathf.Max(moveDuration - elapsedHead, 0f);
            timerText.text = FormatTime(remainingTime);

            yield return null;
        }

        timerText.text = "00:00:00";

        isPrinting = false;
    }
    private float headStartX;
    private float headEndX;
}
