using System;
using UnityEngine;

[Serializable]
public class NewsConfig : IConfig
{
    private string path = "Sprites/News/";

    public string header;
    public string text;
    public int likes;
    public int dislikes;
    public string imageID;

    [SerializeField] private string _id;

    private Sprite _image;

    public Sprite image
    {
        get
        {
            if (_image != null) return _image;

            var sprite = Resources.Load<Sprite>(path + imageID);
            if (sprite != null)
                _image = sprite;

            if (_image == null) Debug.LogWarning($"Icon not found: {imageID}");

            return _image;
        }
        set => _image = value;
    }

    public string id
    {
        get => _id;
        set => _id = value;
    }
}