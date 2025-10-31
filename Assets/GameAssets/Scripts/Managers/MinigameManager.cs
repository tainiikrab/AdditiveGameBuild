using Rellac.Windows;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private GUIWindow slicerWindow;
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
                var window = Instantiate(slicerWindow, laptopUI);
                return;
            case MinigameType.Postprocess:
                SceneSwitchManager.OpenScene(Scenes.PostProcessMinigame);
                return;
            default: return;
        }
    }
}