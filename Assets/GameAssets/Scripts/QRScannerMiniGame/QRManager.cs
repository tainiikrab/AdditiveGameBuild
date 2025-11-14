using System;
using UnityEngine;

public class QRManager : MonoBehaviour
{
    public QRManager Instance {get; private set;}
    
    private ShopItemConfig chosenMaterial;

    private void Awake()
    {
        Instance = this;
    }

    private void ClearChosenMaterial()
    {
        chosenMaterial = null;
    }

    private void AddMaterial(ShopItemConfig material)
    {
        chosenMaterial = material;
    }

    [Serializable]
    public class Boxes
    {
        public GameObject box;
        public ShopItemConfig material;
    }
}
