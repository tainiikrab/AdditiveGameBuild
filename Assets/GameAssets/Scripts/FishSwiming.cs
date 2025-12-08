using UnityEngine;
using DG.Tweening;

public class FishSwiming : MonoBehaviour
{
    [Header("Bobbing Animation")]
    [SerializeField] private float bobbingDistance = 0.02f;
    [SerializeField] private float bobbingDuration = 3f;

    [Header("Random Rotation")]
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;
    [SerializeField] private float rotationAmount = 45f;
    [SerializeField] private float rotationDuration = 0.5f;

    private Vector3 startPosition;
    private Tween bobbingTween;
    private Tween rotationTween;

    private void OnEnable()
    {
        startPosition = transform.position;
        StartBobbing();
        ScheduleRandomRotation();
    }

    private void StartBobbing()
    {
        bobbingTween = transform.DOMoveY(startPosition.y + bobbingDistance, bobbingDuration / 2)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                transform.DOMoveY(startPosition.y - bobbingDistance, bobbingDuration / 2)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => StartBobbing());
            });
    }

    private void ScheduleRandomRotation()
    {
        float waitTime = Random.Range(minWaitTime, maxWaitTime);

        DOVirtual.DelayedCall(waitTime, () =>
        {
            if (gameObject.activeInHierarchy)
            {
                PerformRotation();
                ScheduleRandomRotation();
            }
        });
    }

    private void PerformRotation()
    {
        float randomRotation = Random.value > 0.5f ? rotationAmount : -rotationAmount;

        rotationTween?.Kill();
        rotationTween = transform.DOLocalRotate(
            new Vector3(0, randomRotation, 0),
            rotationDuration,
            RotateMode.LocalAxisAdd
        ).SetEase(Ease.InOutQuad);
    }

    private void OnDisable()
    {
        bobbingTween?.Kill();
        rotationTween?.Kill();
    }
}