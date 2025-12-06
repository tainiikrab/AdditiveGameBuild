using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Scanner : MonoBehaviour
{
    [SerializeField] private RectTransform phoneCameraPoint;
    [SerializeField] private float scanTime = 2f;
    [SerializeField] private Phone phone;
    [SerializeField] private RectTransform scanLine;

    private bool isUIActive;
    private PrintingMaterialConfig currentPrintingMaterial;
    private float scanTimer;
    private Tween scanLineTween;
    private Code currentDetectedCode;

    private void Awake()
    {
        phone.Accepted += OnAccepted;
        phone.Cancelled += OnCancelled;


        if (scanLine != null) scanLine.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isUIActive) Scan();
    }

    private void Scan()
    {
        Code detectedCode = null;
        var isCodeFullyInView = false;

        detectedCode = FindCodeInView();

        if (detectedCode != null) isCodeFullyInView = IsCodeFullyInCameraPoint(detectedCode);

        if (detectedCode == null || !isCodeFullyInView)
        {
            StopScanAnimation();
            scanTimer = 0f;
            currentPrintingMaterial = null;
            currentDetectedCode = null;
            return;
        }

        if (currentDetectedCode != detectedCode)
        {
            currentDetectedCode = detectedCode;

            currentPrintingMaterial = GlobalConfig.Instance.PrintingMaterials.Find(material =>
                material.id.ToString() == currentDetectedCode.MaterialLink.ToString());

            scanTimer = 0f;

            StartScanAnimation();
        }

        scanTimer += Time.deltaTime;

        if (scanTimer >= scanTime)
        {
            StopScanAnimation();
            isUIActive = true;
            phone.Initialize(currentPrintingMaterial);
        }
    }

    private Code FindCodeInView()
    {
        var allCodes = FindObjectsOfType<Code>();

        foreach (var code in allCodes)
            if (IsCodeFullyInCameraPoint(code))
                return code;

        return null;
    }

    private void FinishMinigame()
    {
        SceneSwitchManager.isMinigameFinished = true;
        OrderManager.orderData.chosenMaterial = currentPrintingMaterial;
        currentPrintingMaterial = null;
        SceneSwitchManager.OpenScene(SceneName.MainScene);
    }

    private bool IsCodeFullyInCameraPoint(Code code)
    {
        if (code == null || phoneCameraPoint == null) return false;

        var codeRect = code.GetComponent<RectTransform>();
        if (codeRect == null) return false;

        var codeRectLocal = GetWorldRect(codeRect);
        var cameraRectLocal = GetWorldRect(phoneCameraPoint);

        // Проверяем, полностью ли код внутри CameraPoint
        return cameraRectLocal.Contains(codeRectLocal.min) &&
               cameraRectLocal.Contains(codeRectLocal.max);
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        return new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
        );
    }

    private void StartScanAnimation()
    {
        if (scanLine == null || phoneCameraPoint == null) return;

        StopScanAnimation();

        scanLine.gameObject.SetActive(true);

        var cameraCorners = new Vector3[4];
        phoneCameraPoint.GetWorldCorners(cameraCorners);

        var startPos = new Vector2(
            cameraCorners[0].x,
            (cameraCorners[0].y + cameraCorners[1].y) / 2f
        );

        var endPos = new Vector2(
            cameraCorners[2].x,
            (cameraCorners[0].y + cameraCorners[1].y) / 2f
        );

        scanLine.position = startPos;
        scanLineTween = scanLine.DOMove(endPos, scanTime)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopScanAnimation()
    {
        scanLineTween?.Kill();
        scanLineTween = null;

        if (scanLine != null) scanLine.gameObject.SetActive(false);
    }

    private void OnAccepted()
    {
        isUIActive = false;
        StopScanAnimation();
        phone.ClosePhoneUI();
        // Возврат на главную сцену
        FinishMinigame();
    }

    private void OnCancelled()
    {
        currentPrintingMaterial = null;
        currentDetectedCode = null;
        scanTimer = 0f;
        isUIActive = false;
        StopScanAnimation();
        phone.ClosePhoneUI();
    }

    private void OnDestroy()
    {
        StopScanAnimation();
    }

    /*private void OnDrawGizmos()
    {
        if (phoneCameraPoint != null)
        {
            var corners = new Vector3[4];
            phoneCameraPoint.GetWorldCorners(corners);

            Gizmos.color = Color.green;
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }

            var allCodes = FindObjectsOfType<Code>();
            foreach (var code in allCodes)
            {
                var codeRect = code.GetComponent<RectTransform>();
                if (codeRect != null)
                {
                    var codeCorners = new Vector3[4];
                    codeRect.GetWorldCorners(codeCorners);

                    Gizmos.color = Color.red;
                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.DrawLine(codeCorners[i], codeCorners[(i + 1) % 4]);
                    }
                }
            }
        }
    }*/
}