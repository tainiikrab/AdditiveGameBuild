using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class NewsManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup newsPanel;
    [SerializeField] private TextMeshProUGUI newsText;
    [SerializeField] private TextMeshProUGUI newsHeader;
    [SerializeField] private Image newsImage;

    [SerializeField] private RectTransform teaserPanel;
    [SerializeField] private TextMeshProUGUI teaserHeader;
    [SerializeField] private Image teaserImage;

    private float teaserDefaultPositionY;
    [SerializeField] private float teaserLowerPosition;

    [SerializeField] private Button TeaserButton;

    private void Awake()
    {
        OrderManager.OnOrderFinished += TryLoadNews;
        newsPanel.gameObject.SetActive(false);
        newsPanel.alpha = 0;

        teaserDefaultPositionY = teaserPanel.anchoredPosition.y;
        teaserPanel.anchoredPosition = new Vector2(teaserPanel.anchoredPosition.x, teaserLowerPosition);
        teaserPanel.gameObject.SetActive(false);

        // -----------------------------
        // Set up teaser button effects
        // -----------------------------
        var teaserButtonEvents = TeaserButton.gameObject.AddComponent<TeaserButtonEffects>();
        teaserButtonEvents.Init(ShowNews);
    }

    private void OnDestroy()
    {
        OrderManager.OnOrderFinished -= TryLoadNews;
    }

# if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            var orderData = OrderManager.defaultOrderData;
            TryLoadNews(orderData);
        }
    }
#endif

    private void TryLoadNews(OrderManager.OrderData orderData)
    {
        if (GlobalConfig.Instance.News.Count < orderData.config.plotIndex) return;
        var newsConfig = GlobalConfig.Instance.News[orderData.config.plotIndex - 1];

        if (newsImage == null || teaserImage == null)
        {
            Debug.LogWarning("NewsManager: image reference destroyed.");
            return;
        }

        newsText.text = newsConfig.text;
        newsHeader.text = newsConfig.header;
        newsImage.sprite = newsConfig.image;

        teaserHeader.text = newsConfig.header;
        teaserImage.sprite = newsConfig.image;
        Debug.Log("News loaded");
        ShowTeaser();
    }

    private void ShowTeaser()
    {
        Debug.Log("Show teaser");
        teaserPanel.gameObject.SetActive(true);

        var rt = teaserPanel;

        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, teaserLowerPosition);

        rt.DOAnchorPosY(teaserDefaultPositionY, 0.5f).SetEase(Ease.OutCubic);
    }

    public void HideTeaser()
    {
        var rt = teaserPanel;

        rt.DOAnchorPosY(teaserLowerPosition, 0.4f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => teaserPanel.gameObject.SetActive(false));
    }

    public void ShowNews()
    {
        newsPanel.alpha = 0;
        newsPanel.gameObject.SetActive(true);
        newsPanel.DOFade(1, 0.5f);
        HideTeaser();
    }

    public void HideNews()
    {
        newsPanel.DOFade(0, 0.5f).OnComplete(() => newsPanel.gameObject.SetActive(false));
    }
}


public class TeaserButtonEffects : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rt;
    private System.Action onClickAction;

    private const float normalScale = 1f;
    private const float hoverScale = 1.05f;
    private const float downScale = 1.1f;

    public void Init(System.Action clickCallback)
    {
        rt = GetComponent<RectTransform>();
        onClickAction = clickCallback;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rt.DOScale(hoverScale, 0.15f).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rt.DOScale(normalScale, 0.15f).SetEase(Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rt.DOScale(downScale, 0.1f).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // If still hovering, return to hover scale
        if (eventData.pointerEnter == gameObject)
            rt.DOScale(hoverScale, 0.1f).SetEase(Ease.OutQuad);
        else
            rt.DOScale(normalScale, 0.1f).SetEase(Ease.OutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rt.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.2f, 10, 1f);
        onClickAction?.Invoke();
    }
}