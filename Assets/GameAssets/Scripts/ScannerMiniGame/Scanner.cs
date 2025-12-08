using System.Collections;
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

    // last phone world position for movement detection
    private Vector3 lastPhonePosition;

    // минимальный сдвиг (в мировых координатах) для срабатывания возобновления
    [SerializeField] private float movementThreshold = 0.5f;

    // флаг — разрешать возобновление сканирования при дергании телефона (устанавливается в OnCancelled)
    private bool resumeOnDrag;

    // задержка перед началом отслеживания движения (чтобы избежать ложных толчков сразу после ClosePhoneUI)
    [SerializeField] private float resumeDelay = 0.06f;
    private float resumeDelayTimer;

    private void Awake()
    {
        phone.Accepted += OnAccepted;
        phone.Cancelled += OnCancelled;
        // если Phone больше не бросает ReturnedToStart, можно убрать подписку
        // phone.ReturnedToStart += OnPhoneReturned;

        if (scanLine != null) scanLine.gameObject.SetActive(false);

        // инициализация позиции телефона (безопасно, если phone назначен)
        lastPhonePosition = phone != null ? phone.transform.position : Vector3.zero;
        resumeOnDrag = false;
        resumeDelayTimer = 0f;
    }

    private void Update()
    {
        // если UI активен — ждем начала движения телефона (только если разрешено)
        if (isUIActive)
        {
            if (resumeOnDrag && phone != null)
            {
                // сначала даём короткую паузу, чтобы избежать ложных срабатываний
                if (resumeDelayTimer > 0f)
                {
                    resumeDelayTimer -= Time.deltaTime;
                    // обновляем lastPhonePosition во время паузы, чтобы "зафиксировать" стартовую позицию
                    lastPhonePosition = phone.transform.position;
                    return;
                }

                var currentPos = phone.transform.position;
                var delta = Vector3.Distance(currentPos, lastPhonePosition);

                if (delta > movementThreshold)
                {
                    // началось реальное движение — скрываем UI и возобновляем сканирование
                    isUIActive = false;
                    resumeOnDrag = false;
                    phone.ClosePhoneUI(); // скрыть UI без возврата в позицию (Phone.ClosePhoneUI уже настроен)
                    AudioManager.Instance?.StopSound(SoundType.Scanning);

                    // обновляем базовую позицию и сразу проверяем сканирование в этом кадре
                    lastPhonePosition = currentPos;
                    Scan();
                    return;
                }

                // обновляем позицию для следующей проверки
                lastPhonePosition = currentPos;
            }

            return;
        }

        // если UI не активен — обычный цикл сканирования
        Scan();
    }

    private void Scan()
    {
        Code detectedCode = null;
        var isCodeFullyInView = false;

        detectedCode = FindCodeInView();

        if (detectedCode != null) isCodeFullyInView = IsCodeFullyInCameraPoint(detectedCode);

        if (detectedCode == null || !isCodeFullyInView)
        {
            AudioManager.Instance.StopSound(SoundType.Scanning);
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
            AudioManager.Instance.PlaySound(SoundType.Scanning);
        }

        scanTimer += Time.deltaTime;

        if (scanTimer >= scanTime)
        {
            StopScanAnimation();
            AudioManager.Instance.StopSound(SoundType.Scanning);
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
        // SceneSwitchManager.isMinigameFinished = true;
        OrderManager.orderData.chosenMaterial = currentPrintingMaterial;
        currentPrintingMaterial = null;
        OrderManager.goPrint = true;
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
        StopScanAnimation();
        phone.ClosePhoneUI();
        AudioManager.Instance.PlaySound(SoundType.UniversalClick);
        FinishMinigame();
    }

    private void OnCancelled()
    {
        currentPrintingMaterial = null;
        currentDetectedCode = null;
        scanTimer = 0f;
        AudioManager.Instance.PlaySound(SoundType.Cancel);
        StopScanAnimation();
        // не возвращаем телефон в стартовую позицию — ждем, пока игрок начнёт двигать телефон (но только если нажал continue)
        phone.ClosePhoneUI();

        // фиксируем текущую позицию телефона как стартовую и запускаем "ждущий" режим с короткой паузой
        if (phone != null)
            lastPhonePosition = phone.transform.position;

        resumeDelayTimer = resumeDelay;
        // разрешаем возобновление сканирования при первом движении телефона (после паузы)
        resumeOnDrag = true;
    }

    private void OnPhoneReturned()
    {
        isUIActive = false;
    }

    private void OnDestroy()
    {
        StopScanAnimation();
        if (phone != null)
        {
            phone.Accepted -= OnAccepted;
            phone.Cancelled -= OnCancelled;
            phone.ReturnedToStart -= OnPhoneReturned;
        }
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