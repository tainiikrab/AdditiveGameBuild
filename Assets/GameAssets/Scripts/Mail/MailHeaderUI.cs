using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// DOTween

public class MailHeaderUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI mailName;
    [SerializeField] private MailBodyUI mailBodyPrefab;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = Color.green;

    private Button button;

    public MailUI.Category category { get; set; }

    public MailBodyUI mailBodyUI { get; private set; }
    public OrderConfig orderConfig { get; private set; }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => MailUI.Instance.ActivateHeader(this));
    }

    private void OnDestroy()
    {
        if (mailBodyUI != null)
            Destroy(mailBodyUI.gameObject);
    }

    public void Initialize(OrderConfig orderConfig, RectTransform bodyContainer)
    {
        this.orderConfig = orderConfig;
        if (orderConfig.customerConfig?.icon != null)
            icon.sprite = orderConfig.customerConfig.icon;
        mailName.text = orderConfig.orderName;
        mailBodyUI = Instantiate(mailBodyPrefab, bodyContainer);
        mailBodyUI.Initialize(orderConfig, this);
    }

    // private void TryCacheCanvasGroup()
    // {
    //     if (isCanvasGroupFound)
    //         return;
    //     canvasGroup = GetComponent<CanvasGroup>();
    //     if (canvasGroup == null)
    //         canvasGroup = gameObject.AddComponent<CanvasGroup>();
    //     isCanvasGroupFound = true;
    // }

    public void Toggle(bool toggle)
    {
        transform.DOKill();
        if (button == null) button = GetComponent<Button>();

        if (mailBodyUI == null)
        {
            Debug.LogWarning("idk why but mailBodyUI is null", this);
            return;
        }

        if (toggle)
        {
            transform.localScale = Vector3.one * 0.95f;
            transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

            if (button.targetGraphic != null)
                button.targetGraphic.DOColor(activeColor, 0.25f);

            mailBodyUI.gameObject.SetActive(true);
        }
        else
        {
            if (button.targetGraphic != null)
                button.targetGraphic.DOColor(normalColor, 0.25f);

            mailBodyUI.gameObject.SetActive(false);
        }

        AudioManager.Instance.PlayClickSound();
    }
}