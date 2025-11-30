using System.Collections.Generic;
using UnityEngine;

public class Customer : WayPointFollower
{
    public GameObject modelParent;
    private GameObject modelInstance;
    public CustomerConfig customerConfig { get; private set; }
    public OrderConfig orderConfig { get; private set; }

    private void OnDestroy()
    {
        if (modelInstance != null)
            Destroy(modelInstance);
    }

    public void Initialize(CustomerConfig customerConfig, OrderConfig orderConfig,
        List<IndexActionTriple> indexActionPairs)
    {
        this.orderConfig = orderConfig;
        this.customerConfig = customerConfig;

        foreach (var iap in indexActionPairs)
        {
            Paths[iap.PathIndex].OnPathEnd += iap.action;
            Paths[iap.PathIndex].needStop = iap.needStop;
        }

        if (customerConfig.mesh != null)
        {
            modelInstance = Instantiate(customerConfig.mesh, modelParent.transform.position,
                modelParent.transform.rotation, transform);
            modelInstance.transform.SetParent(modelParent.transform);
        }
    }

    public Sprite GetSprite()
    {
        return customerConfig.icon;
    }

    public string GetName()
    {
        return customerConfig.name;
    }

    public string GetSpeechLine(int index)
    {
        var lines = orderConfig.dialogLines;
        return lines[index];
    }

    public string[] GetAllSpeechLines()
    {
        return orderConfig.dialogLines;
    }

    public string GetAnswer(int index)
    {
        return orderConfig.answerLines[index];
    }

    public string[] GetAllAnswers()
    {
        return orderConfig.answerLines;
    }
}