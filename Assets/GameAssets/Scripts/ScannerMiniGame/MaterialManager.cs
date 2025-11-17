using UnityEngine;
using System;

public class MaterialManager : MonoBehaviour
{
    [SerializeField] private int maxCount;

    private int count;
    
    public MaterialManager Instance { get; set; }

    private void Start()
    {
        //count = SaveManager.gameData.countMaterial;
    }

    public void Replenish()
    {
        count = maxCount;
        //SaveManager.gameData.countMaterial = count;
    }

    public void SpendMaterial(int amount)
    {
        count -= amount;
        //SaveManager.gameData.countMaterial = count;
    }
}
