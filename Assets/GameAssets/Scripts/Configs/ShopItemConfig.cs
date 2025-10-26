using System;
using UnityEngine;

[Serializable]
public class ShopItemConfig : IConfig
{
    private static string[] SpritesPaths =
    {
        "Sprites/Shop/OfferIcons/", "Sprites/SceneObjects/"
    };
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
            if (_icon != null) return _icon;
            
            foreach (var path in SpritesPaths)
            {
                var sprite = Resources.Load<Sprite>(path + iconID);
                if (sprite == null) continue;
                _icon = sprite;
                break;
            }
            
            if (_icon == null)
            {
                Debug.LogWarning($"Icon not found: {iconID} in any of the paths");
            }
            
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