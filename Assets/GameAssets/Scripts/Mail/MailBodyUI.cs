using Rellac.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailBodyUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI requirements;
    [SerializeField] private Button declineButton;
    [SerializeField] private Button acceptButton;

    [SerializeField] private GUIWindow slicerWindow;
    private MailHeaderUI header;
    private OrderConfig order;

    private void OnDestroy()
    {
        // Debug.Log("MailBodyUI destroyed");
    }

    public void Initialize(OrderConfig order, MailHeaderUI header)
    {
        // Debug.Log("Инициализировался");
        this.header = header;
        this.order = order;
        icon.sprite = order.icon;
        title.text = order.orderName;
        description.text = order.description;
        requirements.text = order.requirements;

        // if (declineButton == null) return;
        if (header.category != MailUI.Category.Incoming)
        {
            Destroy(declineButton?.gameObject);
            Destroy(acceptButton?.gameObject);
            return;
        }

        declineButton.onClick.AddListener(DeclineOrder);
        acceptButton.onClick.AddListener(AcceptOrder);

        if (MailUI.currentOrder != null) acceptButton.interactable = false;
    }

    public void AcceptOrder()
    {
        if (MailUI.currentOrder != null)
        {
            Debug.Log("Сначала нужно выполнить текущий заказ.");
            return;
        }

        MailUI.AcceptOrder(order, header);
        Destroy(declineButton.gameObject);
        Destroy(acceptButton.gameObject);
    }

    public void DeclineOrder()
    {
        MailUI.DeclineOrder(order, header);
        Destroy(declineButton.gameObject);
        Destroy(acceptButton.gameObject);
    }
}