using Rellac.Windows;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private GUIWindow slicerWindowPF;
    [SerializeField] private GUIWindow modellingWindowPF;
    [SerializeField] private Transform laptopUI;


    public static MinigameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OpenMinigame(MinigameType minigameType)
    {
        switch (minigameType)
        {
            case MinigameType.Slicer:
                var slicerWindow = Instantiate(slicerWindowPF, laptopUI);
                return;
            case MinigameType.Scanner:
                SceneSwitchManager.OpenScene(SceneName.scannerMiniGame);
                return;
            case MinigameType.Postprocess:
                SceneSwitchManager.OpenScene(SceneName.PostProcessMinigame);
                return;
            case MinigameType.Modelling:
                var modellingWindow = Instantiate(modellingWindowPF, laptopUI);
                return;
            case MinigameType.Scanning:
                SceneSwitchManager.OpenScene(SceneName.ScannerMinigame);
                return;
            case MinigameType.Customization:
                SceneSwitchManager.OpenScene(SceneName.customizationMiniGame);
                return;
            default: return;
        }
    }
}