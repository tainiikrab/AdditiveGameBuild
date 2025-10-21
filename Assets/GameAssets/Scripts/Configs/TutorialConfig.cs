using System;
using UnityEngine;

[Serializable]
public class TutorialConfig : IConfig
{
    public string scene;
    public string text;
    public string answers;
    [SerializeField] private string _id;

    public string id
    {
        get => _id;
        set => _id = value;
    }
}