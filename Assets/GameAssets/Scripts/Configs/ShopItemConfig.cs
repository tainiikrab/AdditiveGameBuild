using System;
using UnityEngine;

[Serializable]
public class ShopItemConfig : IConfig
{
    private const string SpritesPath = "Sprites/Shop/OfferIcons/";
    private const string PrefabPath = "Models/Shop/";
    public string name;
    public string iconID;
    public string prefabID;
    public int price;
    public string category;
    public string description;

    [SerializeField] private string _id;

    private Sprite _icon;
    private GameObject _prefab;

    public Sprite Icon
    {
        get
        {
            if (_icon == null)
                _icon = Resources.Load<Sprite>(SpritesPath + iconID);
            return _icon;
        }
        set => _icon = value;
    }

    public GameObject Prefab
    {
        get
        {
            if (_prefab == null)
                _prefab = Resources.Load<GameObject>(PrefabPath + prefabID);
            return _prefab;
        }
        set => _prefab = value;
    }

    public string id
    {
        get => _id;
        set => _id = value;
    }
}