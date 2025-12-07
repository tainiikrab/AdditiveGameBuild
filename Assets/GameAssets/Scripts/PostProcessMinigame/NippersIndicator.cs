using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NippersIndicator : MonoBehaviour
{
    [Header("Transforms")] [SerializeField]
    private RectTransform movingBall;

    [SerializeField] private RectTransform triangle;

    [Header("Optional UI Images (for color feedback)")] [SerializeField]
    private Image ballImage; // optional: assign if you want color feedback

    [SerializeField] private Image triangleImage; // optional: assign if you want color feedback

    [Header("Movement")] [SerializeField] private float positionLimit = 0.2f;

    [Header("Speed")] [SerializeField] private float minSpeed = 0.01f;
    [SerializeField] private float maxSpeed = 0.05f;

    [Header("Appearance")] [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField] private float freezeTime = 0.2f;

    [Header("Accuracy thresholds (0..1)")] [Range(0f, 1f)] [SerializeField]
    private float perfectThreshold = 0.9f;

    [Range(0f, 1f)] [SerializeField] private float goodThreshold = 0.65f;
    [Range(0f, 1f)] [SerializeField] private float poorThreshold = 0.35f;

    private WaitForSeconds wait;
    private float currentSpeed;
    private int direction = 1; // 1 = right, -1 = left
    private bool isFrozen = false;

    private Tween idleScaleTween;

    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0;

        PickNewSpeed();
        NippersTool.OnTargetChanged += PickNewSpeed;
        NippersTool.OnNippersUse += FreezeBall;
        wait = new WaitForSeconds(freezeTime);

        // ADDED ANIMATION: small idle bob (store tween so we can pause/kill and restart)
        idleScaleTween = movingBall.DOScale(1.05f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        NippersTool.OnTargetChanged -= PickNewSpeed;
        NippersTool.OnNippersUse -= FreezeBall;

        idleScaleTween?.Kill();
    }

    private void Update()
    {
        if (isFrozen) return;

        if (!NippersTool.isCutting)
        {
            if (canvasGroup != null && canvasGroup.alpha >= 0.99f)
                canvasGroup.DOFade(0, 0.2f);
            return;
        }

        if (canvasGroup != null && canvasGroup.alpha <= 0.01f)
            canvasGroup.DOFade(1, 0.2f);

        MoveBall();
    }

    private void MoveBall()
    {
        var newX = movingBall.anchoredPosition.x + direction * currentSpeed * Time.deltaTime;
        movingBall.anchoredPosition = new Vector2(newX, movingBall.anchoredPosition.y);

        if (newX >= positionLimit)
            direction = -1;
        else if (newX <= -positionLimit) direction = 1;
    }

    // Called on Nippers use
    private void FreezeBall()
    {
        // compute accuracy BEFORE freezing
        var accuracy = ComputeAccuracy(); // 0..1, 1 == perfectly centered (x == 0)

        isFrozen = true;

        // Stop idle scale tween (we'll restart in UnfreezeBall)
        idleScaleTween?.Kill();
        idleScaleTween = null;

        // ----------------------------------------------------------
        // FEEDBACK depending on accuracy
        // ----------------------------------------------------------
        ApplyFeedback(accuracy);
        PostprocessMinigame.removedSupports += accuracy * 1.3f;
        Debug.Log("Accuracy: " + accuracy);
        Debug.Log("Removed supports: " + PostprocessMinigame.removedSupports);
        StartCoroutine(UnfreezeBall());
    }

    private IEnumerator UnfreezeBall()
    {
        yield return wait;
        isFrozen = false;

        // restart idle bob (small)
        idleScaleTween = movingBall.DOScale(1.05f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void PickNewSpeed()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }

    /// <summary>
    /// Compute accuracy: 1.0 when x == 0, 0 when |x| >= positionLimit.
    /// If positionLimit is zero, returns 0 (avoid div by zero).
    /// </summary>
    private float ComputeAccuracy()
    {
        if (positionLimit <= 0f) return 0f;
        var dist = Mathf.Abs(movingBall.anchoredPosition.x);
        var normalized = Mathf.Clamp01(1f - dist / positionLimit); // 1 at center, 0 at edges
        return normalized;
    }

    /// <summary>
    /// Apply visuals scaled by accuracy (0..1).
    /// Higher accuracy -> stronger positive feedback (bigger punch, greenish); lower -> weaker + reddish + shake.
    /// </summary>
    private void ApplyFeedback(float accuracy)
    {
        // kill existing tweens on those transforms/images so feedback doesn't stack weirdly
        movingBall.DOKill();
        triangle.DOKill();
        if (ballImage) ballImage.DOKill();
        if (triangleImage) triangleImage.DOKill();

        // choose feedback tier
        if (accuracy >= perfectThreshold)
        {
            // PERFECT: big punch, small extra pop, green flash on triangle
            var punchStrength =
                Mathf.Lerp(0.18f, 0.28f,
                    (accuracy - perfectThreshold) / (1f - perfectThreshold)); // stronger for super-perfect
            movingBall.DOPunchScale(new Vector3(punchStrength, punchStrength, 0f), 0.28f, 12, 0.8f);
            movingBall.DOShakeRotation(0.25f, new Vector3(0, 0, 8f), 8, 90f); // slight rotational flair

            triangle.DOScale(1.25f, 0.09f).SetLoops(2, LoopType.Yoyo);
            triangle.DOShakeRotation(0.25f, new Vector3(0, 0, 12f), 10, 90f);

            // color feedback (if images provided)
            if (triangleImage)
            {
                var old = triangleImage.color;
                triangleImage.DOColor(Color.green, 0.06f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => triangleImage.color = old);
            }

            if (ballImage)
            {
                var old = ballImage.color;
                ballImage.DOColor(new Color(0.6f, 1f, 0.6f), 0.06f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => ballImage.color = old);
            }
        }
        else if (accuracy >= goodThreshold)
        {
            // GOOD: medium punch, greenish-yellow flash
            var punchStrength =
                Mathf.Lerp(0.12f, 0.18f, (accuracy - goodThreshold) / (perfectThreshold - goodThreshold));
            movingBall.DOPunchScale(new Vector3(punchStrength, punchStrength, 0f), 0.22f, 10, 0.6f);

            triangle.DOScale(1.18f, 0.08f).SetLoops(2, LoopType.Yoyo);
            triangle.DORotate(new Vector3(0, 0, 8f), 0.1f).SetLoops(2, LoopType.Yoyo);

            if (triangleImage)
            {
                var old = triangleImage.color;
                triangleImage.DOColor(new Color(1f, 0.9f, 0.4f), 0.06f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => triangleImage.color = old);
            }
        }
        else if (accuracy >= poorThreshold)
        {
            // POOR: small punch, slight red tint
            var punchStrength = Mathf.Lerp(0.06f, 0.12f, (accuracy - poorThreshold) / (goodThreshold - poorThreshold));
            movingBall.DOPunchScale(new Vector3(punchStrength, punchStrength, 0f), 0.18f, 8, 0.5f);

            // small shake to indicate inaccuracy
            movingBall.DOShakeScale(0.18f, 0.06f, 8);

            triangle.DOScale(1.08f, 0.07f).SetLoops(2, LoopType.Yoyo);

            if (triangleImage)
            {
                var old = triangleImage.color;
                triangleImage.DOColor(new Color(1f, 0.7f, 0.5f), 0.06f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => triangleImage.color = old);
            }
        }
        else
        {
            // MISS: weak punch but heavy shake + red flash
            movingBall.DOPunchScale(new Vector3(0.04f, 0.04f, 0f), 0.18f, 6, 0.4f);
            movingBall.DOShakeScale(0.28f, 0.12f, 18);
            movingBall.DOShakeRotation(0.28f, new Vector3(0, 0, 20f), 18, 90f);

            triangle.DOScale(0.9f, 0.12f).SetLoops(2, LoopType.Yoyo); // small shrink as negative feedback
            triangle.DORotate(new Vector3(0, 0, -18f), 0.18f).SetLoops(2, LoopType.Yoyo);

            if (triangleImage)
            {
                var old = triangleImage.color;
                triangleImage.DOColor(Color.red, 0.08f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => triangleImage.color = old);
            }

            if (ballImage)
            {
                var old = ballImage.color;
                ballImage.DOColor(new Color(1f, 0.5f, 0.5f), 0.08f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => ballImage.color = old);
            }
        }
    }
}