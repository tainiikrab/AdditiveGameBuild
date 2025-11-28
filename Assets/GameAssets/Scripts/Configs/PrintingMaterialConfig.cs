using UnityEngine;
using System;

[Serializable]
public class PrintingMaterialConfig : IConfig
{
    private static string IconsPath = "Sprites/PrintingMaterials/Icons/";

    public string name;
    public string goodProperties;
    public string badProperties;
    public string iconID;

    private string[] _goodProperties;
    private string[] _badProperties;
    private Sprite _icon;

    public string[] GoodProperties
    {
        get
        {
            if (_goodProperties == null || _goodProperties.Length == 0)
            {
                _goodProperties = goodProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < _goodProperties.Length; i++)
                {
                    _goodProperties[i] = _goodProperties[i].Trim();
                }
            }
            return _goodProperties;
        }
    }

    public string[] BadProperties
    {
        get
        {
            if (_badProperties == null || _badProperties.Length == 0)
            {
                _badProperties = badProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < _badProperties.Length; i++)
                {
                    _badProperties[i] = _badProperties[i].Trim();
                }
            }
            return _badProperties;
        }
    }

    public Sprite Icon
    {
        get
        {
            if (_icon == null)
            {
                _icon = Resources.Load<Sprite>(IconsPath + iconID);
            }
            return _icon;
        }
        set => _icon = value;
    }

    [SerializeField] private string _id;

    public string id
    {
        get => _id;
        set => _id = value;
    }
}