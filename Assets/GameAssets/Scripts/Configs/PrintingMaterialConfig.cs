using UnityEngine;
using System;

[Serializable]
public class PrintingMaterialConfig : IConfig
{
    private static string IconsPath = "Sprites/PrintingMaterials/Icons/";
    private char delimiter = ',';

    public string name;
    public string goodProperties;
    public string badProperties;
    public string iconID;

    private string[] _goodProperties;
    private string[] _badProperties;
    private Sprite _icon;

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