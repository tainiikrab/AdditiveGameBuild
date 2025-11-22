using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [SerializeField] private int maxCount;
    [SerializeField] private MaterialWarningUI materialWarningUI;

    private int count;
    
    public int Count
    {
        get => count;
        private set
        {
            if (count == 0)
            {
                ShowWarning();
            }
            else
            {
                count = value;
            }
            SaveManager.gameData.materialCount = count;
        }
    }
    
    public static MaterialManager Instance { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (!PlayerPrefs.HasKey("materials_initialized"))
        {
            Count = maxCount;
            PlayerPrefs.SetInt("materials_initialized", 1);
        }
        else
        {
            Count = SaveManager.gameData.materialCount;
        }
    }

    private void Start()
    {
        Count = SaveManager.gameData.materialCount;
    }

    public void Replenish()
    {
        Count = maxCount;
    }

    public void SubtractCount(int amount)
    {
        Count = count - amount;
    }

    private void ShowWarning()
    {
        materialWarningUI.gameObject.SetActive(true);
    }
}
