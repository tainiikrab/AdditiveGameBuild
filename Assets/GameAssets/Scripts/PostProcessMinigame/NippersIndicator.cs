using System.Collections;
using DG.Tweening;
using UnityEngine;

public class NippersIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform movingBall;
    [SerializeField] private RectTransform triangle;

    [SerializeField] private float positionLimit = 0.2f;

    [SerializeField] private float minSpeed = 0.01f;
    [SerializeField] private float maxSpeed = 0.05f;

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float freezeTime = 0.2f;

    private WaitForSeconds wait;
    private float currentSpeed;
    private int direction = 1; // 1 = right, -1 = left

    private void Awake()
    {
        canvasGroup.alpha = 0;
        PickNewSpeed();
        NippersTool.OnTargetChanged += PickNewSpeed;
        NippersTool.OnNippersUse += FreezeBall;
        wait = new WaitForSeconds(freezeTime);
    }

    private void OnDestroy()
    {
        NippersTool.OnTargetChanged -= PickNewSpeed;
        NippersTool.OnNippersUse -= FreezeBall;
    }

    private bool isFrozen = false;

    private void Update()
    {
        if (isFrozen) return;
        if (!NippersTool.isCutting)
        {
            if (canvasGroup.alpha >= 0.99f)
                canvasGroup.DOFade(0, 0.2f);
            return;
        }

        if (canvasGroup.alpha <= 0.01f)
            canvasGroup.DOFade(1, 0.2f);

        MoveBall();
    }

    private void MoveBall()
    {
        var newX = movingBall.anchoredPosition.x + direction * currentSpeed * Time.deltaTime;
        Debug.Log(newX);
        movingBall.anchoredPosition = new Vector2(newX, movingBall.anchoredPosition.y);

        if (newX >= positionLimit)
            direction = -1;
        else if (newX <= -positionLimit) direction = 1;
    }

    private void FreezeBall()
    {
        isFrozen = true;
        StartCoroutine(UnfreezeBall());
    }

    private IEnumerator UnfreezeBall()
    {
        yield return wait;
        isFrozen = false;
    }

    private void PickNewSpeed()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }
}