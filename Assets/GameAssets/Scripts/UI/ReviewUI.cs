using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void Initialize(OrderConfig order)
    {
        reward = Mathf.Max(100, order.reward);
        Debug.Log($"Default reward: {order.reward}");

        var score = Mathf.Max(0, Mathf.CeilToInt(OrderManager.orderData.quality.totalQuality));
        InitStars(Mathf.RoundToInt(score / 20f));
        Debug.Log($"Score: {score}");
        Debug.Log($"Quality: {OrderManager.orderData.quality.totalQuality}");
        reward = Mathf.Max(0, Mathf.CeilToInt(reward * (score / 90f)));
        rewardAmountLabel.text = reward.ToString();

        if (order.customerConfig.icon != null)
            customerImage.sprite = order.customerConfig.icon;
        customerName.text = order.customerConfig.name;
        orderName.text = order.orderName;


        var index = Mathf.RoundToInt(score / 20f) - 1;
        index = Mathf.Max(0, index);
        index = Mathf.Min(reviewTexts.Length - 1, index);
        reviewLabel.text = reviewTexts[index];
    }

    private void InitStars(int grade)
    {
        foreach (Transform child in starsContainer)
        {
            if (grade <= 0) return;
            child.GetComponent<Image>().sprite = filledStar;
            grade--;
        }
    }

    public void Hide()
    {
        GameManager.Instance.points += reward;
        OrderManager.CompleteOrder();
        gameObject.SetActive(false);
        AudioManager.Instance.PlayClickSound();
    }
}