using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IndexActionTriple
{
    public int PathIndex;
    public bool needStop = true;
    public Action action;
}

public class CustomerManager : MonoBehaviour
{
    [Header("Customer Settings")] 
    public Customer customer;
    public DialogUI dialogUI;

    private Customer activeCustomer;
    private GameManager gm;

    private void Awake()
    {
        OrderManager.OnOrderPlotCreated += CreateCustomer;
        OrderManager.OnOrderFinished += OnOrderComplete;
        customer.StopMovement();
    }

    private void OnDestroy()
    {
        OrderManager.OnOrderPlotCreated -= CreateCustomer;
        OrderManager.OnOrderFinished -= OnOrderComplete;
    }

    public void CreateCustomer(OrderConfig orderConfig)
    {
        var customerConfig = orderConfig.customerConfig;
        dialogUI.ToggleVisibility(false);

        dialogUI.OnOrderAccepted += OnOrderAccepted;
        dialogUI.OnOrderRejected += OnOrderRejected;
        dialogUI.OnDialogClosed += HandleDialogClosed;

        activeCustomer = customer;

        var pair = new IndexActionTriple { PathIndex = 0, action = ShowDialogueUI };
        activeCustomer.Initialize(customerConfig, orderConfig, new List<IndexActionTriple> { pair });

        activeCustomer.StartMovement();
        Debug.Log($"🧍 Клиент '{customerConfig.name}' создан");
    }

    private void ShowDialogueUI()
    {
        dialogUI.StartDialog(
            activeCustomer.GetName(),
            activeCustomer.GetSprite(),
            activeCustomer.GetAllSpeechLines(),
            activeCustomer.GetAllAnswers()
        );
    }

    private void HandleDialogClosed()
    {
        activeCustomer.ContinueMovement();
    }

    private void OnOrderAccepted()
    {
        gm.OnOrderAccepted();
        activeCustomer.ContinueMovement();
    }

    private void OnOrderRejected()
    {
        gm.OnOrderRejected();
        activeCustomer.ContinueMovement();
    }

    private void OnOrderComplete()
    {
        gm.OnOrderComplete();
    }
}
