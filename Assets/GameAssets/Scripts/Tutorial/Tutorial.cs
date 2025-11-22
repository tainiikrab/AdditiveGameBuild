using UnityEngine;
using System;
using UnityEngine.Rendering;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Tutorial UIs")] [SerializeField]
    private CanvasGroup tutorialUI;

    [SerializeField] private GameObject workplaceTutorialUI;
    [SerializeField] private GameObject laptopTutorialUI;


    [Space(10)] [Header("Workplace")] [SerializeField]
    private Light workplaceLight;

    [SerializeField] private CinemachineCamera workplaceCamera;

    [Space(10)] [Header("Laptop")] [SerializeField]
    private LaptopTrigger laptopBlinker;

    [SerializeField] private LaptopUI laptopUI;
    [SerializeField] private RectTransform mailImage;
    [SerializeField] private RectTransform shopImage;
    [SerializeField] private RectTransform closeImage;

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


        workplaceTutorialUI.gameObject.SetActive(true);
        laptopTutorialUI.gameObject.SetActive(false);
        tutorialUI.alpha = 0;
        tutorialUI.gameObject.SetActive(false);

        StartTutorial();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) StartTutorial();
        if (Input.GetKeyDown(KeyCode.Y)) EndTutorial();
    }
    // public void Start()
    // {
    //     laptopUI.OnVisibilityChanged() +=  
    // }

    public void StartTutorial()
    {
        tutorialUI.gameObject.SetActive(true);
        tutorialUI.DOFade(1, 0.5f);
        isTutorialActive = true;
    }

    public void EndTutorial()
    {
        HideWorkplaceTutorial();
        isTutorialActive = false;
        tutorialUI.DOFade(0, 0.5f).OnComplete(() => tutorialUI.gameObject.SetActive(false));
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


    public void SwitchScene(int sceneInt)
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
                highlightTween?.Kill();

                mailImage.transform.DOLocalRotate(Vector3.zero, 0.5f)
                    .SetEase(Ease.OutElastic);
                shopImage.transform.DOLocalRotate(Vector3.zero, 0.5f)
                    .SetEase(Ease.OutElastic);

                break;
            case TutorialScene.Workplace:
                ShowWorkplaceScene();
                currentScene = TutorialScene.Workplace;
                break;

            case TutorialScene.BlinkingLaptop:
                laptopUI.OnVisibilityChanged +=
                    LaptopTutorial;
                tutorialUI.DOFade(0, 0.5f).OnComplete(() => tutorialUI.gameObject.SetActive(false));
                laptopBlinker.isBlinking = true;
                currentScene = TutorialScene.BlinkingLaptop;
                break;
            case TutorialScene.MailHighlight:
                currentScene = TutorialScene.MailHighlight;

                highlightTween = DOTween.Sequence()
                    .Append(mailImage.DOLocalRotate(new Vector3(0, 0, 15), 0.5f).SetEase(Ease.InOutSine))
                    .Append(mailImage.DOLocalRotate(new Vector3(0, 0, -15), 0.5f).SetEase(Ease.InOutSine))
                    .Append(mailImage.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutSine))
                    .SetLoops(-1);
                break;
            case TutorialScene.ShopHighlight:
                currentScene = TutorialScene.ShopHighlight;
                highlightTween?.Kill();
                mailImage.transform.DOLocalRotate(Vector3.zero, 0.5f)
                    .SetEase(Ease.OutElastic);

                highlightTween = DOTween.Sequence()
                    .Append(shopImage.DOLocalRotate(new Vector3(0, 0, 15), 0.5f).SetEase(Ease.InOutSine))
                    .Append(shopImage.DOLocalRotate(new Vector3(0, 0, -15), 0.5f).SetEase(Ease.InOutSine))
                    .Append(shopImage.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutSine))
                    .SetLoops(-1);
                break;
            case TutorialScene.CloseLaptop:
                currentScene = TutorialScene.CloseLaptop;
                highlightTween?.Kill();
                shopImage.transform.DOLocalRotate(Vector3.zero, 0.5f)
                    .SetEase(Ease.OutElastic);

                highlightTween = DOTween.Sequence()
                    .Append(closeImage.DOLocalRotate(new Vector3(0, 0, 15), 0.5f).SetEase(Ease.InOutSine))
                    .Append(closeImage.DOLocalRotate(new Vector3(0, 0, -15), 0.5f).SetEase(Ease.InOutSine))
                    .Append(closeImage.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutSine))
                    .SetLoops(-1);
                tutorialUI.gameObject.SetActive(false);
                break;
            case TutorialScene.CallCustomer:
                currentScene = TutorialScene.CallCustomer;
                highlightTween?.Kill();
                OrderManager.CreatePlotOrder(1);
                break;
        }
    }

    private Tween highlightTween;


    private void LaptopTutorial(bool isVisible)
    {
        if (isVisible)
        {
            laptopTutorialUI.SetActive(true);
            tutorialUI.gameObject.SetActive(true);
            tutorialUI.DOFade(1, 0.5f);
            laptopTutorialUI.SetActive(true);
            SwitchScene((int)TutorialScene.None);
        }
        else if (currentScene == TutorialScene.CloseLaptop)
        {
            laptopUI.OnVisibilityChanged -= LaptopTutorial;
            SwitchScene((int)TutorialScene.CallCustomer);
        }
    }

    public enum TutorialScene
    {
        None, // 0
        Workplace, // 1
        BlinkingLaptop, // 2
        MailHighlight, // 3
        ShopHighlight, // 4
        CloseLaptop, // 5
        CallCustomer, // 6
        end
    }
}