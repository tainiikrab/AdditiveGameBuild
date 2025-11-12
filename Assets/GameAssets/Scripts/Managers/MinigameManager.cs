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
            case MinigameType.Postprocess:
                SceneSwitchManager.OpenScene(Scenes.PostProcessMinigame);
                return;
            case MinigameType.Modelling:
                var modellingWindow = Instantiate(modellingWindowPF, laptopUI);
                return;
            default: return;
        }
    }
}