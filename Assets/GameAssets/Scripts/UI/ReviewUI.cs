using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ReviewUI : MonoBehaviour
{
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite filledStar;

    [SerializeField] private Transform starsContainer;

    [SerializeField] private Image customerImage;
    [SerializeField] private TextMeshProUGUI customerName;
    [SerializeField] private TextMeshProUGUI orderName;
    [SerializeField] private TextMeshProUGUI rewardAmountLabel;
    [SerializeField] private TextMeshProUGUI reviewLabel;

    [SerializeField] private int reward;
    [SerializeField] private Button button;

    [SerializeField] private string[] reviewTexts;

    public CanvasGroup canvasGroup;

    [Space(10)] [Header("Misc")] [SerializeField]
    private int defaultScore;

    public void Initialize()
    {
        var orderData = OrderManager.orderData;

        int score;
        if (orderData == null)
        {
            orderData = OrderManager.defaultOrder;
            score = defaultScore;
        }
        else
        {
            score = Mathf.Max(0, Mathf.CeilToInt(orderData.quality.totalQuality));
        }

        reward = Mathf.Max(100, orderData.config.reward);
        // Debug.Log($"Default reward: {orderData.config.reward}");

        for (var i = 0; i < starsContainer.childCount; i++)
        {
            var star = starsContainer.GetChild(i);
            star.localScale = Vector3.zero;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOFade(1, 0.5f).OnComplete(() => InitStars(Mathf.RoundToInt(score / 20f)));

        Debug.Log($"Score: {score}");
        Debug.Log($"Quality: {orderData.quality.totalQuality}");
        reward = Mathf.Max(0, Mathf.CeilToInt(reward * (score / 90f)));
        rewardAmountLabel.text = reward.ToString();

        if (orderData.config.customerConfig.icon != null)
            customerImage.sprite = orderData.config.customerConfig.icon;
        customerName.text = orderData.config.customerConfig.name;
        orderName.text = orderData.config.orderName;


        var index = Mathf.RoundToInt(score / 20f) - 1;
        index = Mathf.Max(0, index);
        index = Mathf.Min(reviewTexts.Length - 1, index);
        reviewLabel.text = reviewTexts[index];
    }

    private void InitStars(int grade)
    {
        for (var i = 0; i < starsContainer.childCount; i++)
        {
            var star = starsContainer.GetChild(i);
            var starImage = star.GetComponent<Image>();

            if (i < grade)
                starImage.sprite = filledStar;
            star.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.1f * i);

            // else
            // {
            //     star.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(0.1f * i);
            // }
        }
    }

    public void Hide()
    {
        //AudioManager.Instance.PlaySound(SoundType.Close);
        GameManager.Instance.points += reward;
        OrderManager.CompleteOrder();
        OrderManager.CreateRegularOrder(3);

        canvasGroup.DOFade(0, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
    }
}