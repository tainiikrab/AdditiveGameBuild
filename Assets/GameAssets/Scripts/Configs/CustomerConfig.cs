using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomerConfig : IConfig
{
    private const string spritesPath = "Sprites/Customers/";
    private const string meshPath = "_StaticModels/Bears/";
    public string name;
    public string iconID;
    public string modelID;

    // public List<OrderConfig> availableOrders
    // {
    //     get
    //     {
    //         if (_orders == null)
    //         {
    //             _orders = new List<OrderConfig>();
    //             foreach (var plotOrder in GameManager.globalConfig.Orders)
    //                 if (plotOrder.customerID == id)
    //                     _orders.Add(plotOrder);
    //         }
    //
    //         return _orders;
    //     }
    //     private set => _orders = value;
    // }

    [SerializeField] private string _id;


    private Sprite _icon;

    private GameObject _mesh;
    // private List<OrderConfig> _orders;

    public Sprite icon
    {
        get
        {
            if (_icon == null)
                _icon = Resources.Load<Sprite>(spritesPath + iconID);
            return _icon;
        }
        private set => _icon = value;
    }

    public GameObject mesh
    {
        get
        {
            if (_mesh == null)
                _mesh = Resources.Load<GameObject>(meshPath + modelID);
            return _mesh;
        }
        private set => _mesh = value;
    }

    public string id
    {
        get => _id;
        set => _id = value;
    }
}