using UnityEngine;
using DG.Tweening;

public abstract class AbstractTool : MonoBehaviour
{
    [Header("Follow Settings")] [SerializeField]
    protected float followSpeed = 15f;

    [Header("Return Settings")] [SerializeField]
    protected float returnDuration = 0.5f;

    [SerializeField] protected Ease returnEase = Ease.OutElastic;
    [SerializeField] protected float elasticity = 1f;
    [SerializeField] protected float oscillations = 0.3f;

    protected bool isDragging;
    protected Tweener moveTween;
    protected Vector3 startPosition;

    protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] sounds;

    protected virtual void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        OnStopUse();
    }

    protected virtual void Update()
    {
        if (isDragging)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(worldPos.x, worldPos.y, startPosition.z),
                Time.deltaTime * followSpeed
            );

            // Debug.Log("Abstract");
            OnUse();
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
        moveTween?.Kill();
    }

    private void OnMouseUp()
    {
        isDragging = false;
        OnStopUse();
        moveTween = transform.DOMove(startPosition, returnDuration)
            .SetEase(returnEase, elasticity, oscillations);
    }

    /// <summary>
    /// Метод действия инструмента — переопределяется в наследниках
    /// </summary>
    protected abstract void OnUse();

    protected virtual void OnStopUse()
    {
    }
}