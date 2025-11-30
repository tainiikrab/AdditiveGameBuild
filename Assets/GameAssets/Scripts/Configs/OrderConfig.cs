using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class OrderConfig : IConfig
{
    private const string SpritesPath = "Sprites/UISprites/DesktopIcons/";
    private const string ModelsPath = "Models/PrintableObjects/";
    public string orderName;
    public string description;
    public string requirements;
    public string customerID;
    public string rawDialogLines;

    public string rawAnswerLines;
    public int plotIndex;
    public string iconID;
    public bool isMail;


    public float layerHeight;
    public float fillDensity;
    public float printSpeed;
    public float sandpapering;

    public string meshID;
    public int reward;


    [SerializeField] private string _id;

    private string[] _answerLines;


    private CustomerConfig _customer;

    private string[] _dialogLines;
    private Sprite _icon;

    private GameObject _mesh;

    private List<ShopItemConfig> _requiredItems;


    public string rawPrinterTypeName;
    public PrinterType printerType => PrinterData.Types[rawPrinterTypeName];

    public string[] dialogLines
    {
        get
        {
            if (_dialogLines == null) _dialogLines = rawDialogLines.Split('\n');
            return _dialogLines;
        }
        private set => _dialogLines = value;
    }

    public string[] answerLines
    {
        get
        {
            if (_answerLines == null) _answerLines = rawAnswerLines.Split('\n');
            return _answerLines;
        }
        private set => _answerLines = value;
    }

    public bool wasCalled { get; private set; }

    public CustomerConfig customerConfig
    {
        get
        {
            if (_customer == null)
                _customer = GlobalConfig.Instance.Customers.Find(foundCustomer =>
                    foundCustomer.id == customerID);
            return _customer;
        }
        private set => _customer = value;
    }

    public Sprite icon
    {
        get
        {
            if (_icon == null)
                _icon = Resources.Load<Sprite>(SpritesPath + iconID);
            return _icon;
        }
        set => _icon = value;
    }

    public GameObject mesh
    {
        get
        {
            if (_mesh == null)
                _mesh = Resources.Load<GameObject>(ModelsPath + id);
            return _mesh;
        }
        set => _mesh = value;
    }

    public List<ShopItemConfig> requiredItems
    {
        get
        {
            if (_requiredItems == null)
            {
                var split = requirements.Split(',');
                _requiredItems = new List<ShopItemConfig>();

                foreach (var item in GlobalConfig.Instance.ShopItems)
                {
                    var requiredItem = Array.Find(split, str => str == item.id);
                    _requiredItems.Add(
                        GlobalConfig.Instance.ShopItems.Find(shopItem => requiredItem == shopItem.id));
                }
            }

            return _requiredItems;
        }
        set => _requiredItems = value;
    }

    public string id
    {
        get => _id;
        set => _id = value;
    }

    public void SetWasCalled(bool value)
    {
        wasCalled = value;
    }

}