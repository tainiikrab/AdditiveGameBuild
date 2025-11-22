using System;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Unity.Cinemachine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private CanvasGroup tutorialUI;

    [Space(10)] [Header("Workplace")] [SerializeField]
    private Light workplaceLight;

    [SerializeField] private CinemachineCamera workplaceCamera;

    [Space(10)] [Header("Laptop")] [SerializeField]
    private LaptopTrigger laptopBlinker;

    [SerializeField] private LaptopUI laptopUI;

    public static Tutorial instance { get; private set; }

    private float workplaceLightIntensity;

    public static bool isTutorialActive = false;
    public TutorialScene currentScene = TutorialScene.None;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (!workplaceLight.gameObject.activeSelf) workplaceLight.gameObject.SetActive(true);
        workplaceLightIntensity = workplaceLight.intensity;
        workplaceLight.intensity = 0;
        tutorialUI.alpha = 0;
        tutorialUI.gameObject.SetActive(false);
    }

    // public void Start()
    // {
    //     laptopUI.OnVisibilityChanged() +=  
    // }

    public void StartTutorial()
    {
        tutorialUI.gameObject.SetActive(true);
        tutorialUI.DOFade(1, 1f);
        isTutorialActive = true;
    }

    public void EndTutorial()
    {
        HideWorkplaceTutorial();
        isTutorialActive = false;
        tutorialUI.DOFade(0, 1f).OnComplete(() => tutorialUI.gameObject.SetActive(false));
    }


    private void ShowWorkplaceScene()
    {
        workplaceLight.DOIntensity(workplaceLightIntensity, 1f);
        workplaceCamera.Priority = 100;
        // tutorialUI.gameObject.SetActive(true);
        // tutorialUI.DOFade(1, 1f);
    }

    private void HideWorkplaceTutorial()
    {
        workplaceLight.DOIntensity(0, 1f);
        workplaceCamera.Priority = -100;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) StartTutorial();
        if (Input.GetKeyDown(KeyCode.Y)) EndTutorial();
    }

    public void HighlightScene(int sceneInt)
    {
        if (sceneInt < 0 || sceneInt >= Enum.GetValues(typeof(TutorialScene)).Length)
            return;

        var scene = (TutorialScene)sceneInt;

        switch (scene)
        {
            case TutorialScene.None:
                HideWorkplaceTutorial();
                laptopBlinker.isBlinking = false;
                currentScene = TutorialScene.None;
                break;
            case TutorialScene.Workplace:
                ShowWorkplaceScene();
                currentScene = TutorialScene.Workplace;
                break;

            case TutorialScene.BlinkingLaptop:
                laptopBlinker.isBlinking = true;
                tutorialUI.gameObject.SetActive(false);
                currentScene = TutorialScene.BlinkingLaptop;

                break;
        }
    }

    public enum TutorialScene
    {
        None, // 0
        Workplace, // 1
        BlinkingLaptop, // 2
        BlinkingMail // 3
    }
}