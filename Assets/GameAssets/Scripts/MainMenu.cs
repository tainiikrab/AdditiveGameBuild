using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject otherUIs;
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private CinemachineCamera vcam;

    [SerializeField] private MouseRaycaster mouseRaycaster;

    [SerializeField] private float creditsSpeed = 0.1f;

    [SerializeField] private GameObject ui;
    [SerializeField] private CanvasGroup uiContainer;
    [SerializeField] private CanvasGroup creditsCanvasGroup;

    [SerializeField] private Tutorial tutorial;

    private RectTransform creditsBody;

    private static bool isGameLaunch = true;
    private bool isMenuOpen = false;

    private void Awake()
    {
        if (!isGameLaunch)
        {
            StartGame();
            ui.SetActive(false);
            return;
        }

        creditsBody = creditsCanvasGroup.transform.GetChild(0).GetComponent<RectTransform>();
        ShowMainMenu();
        Time.timeScale = 1;
    }

    public void ShowMainMenu()
    {
        vcam.Priority = 100;
        ui.SetActive(true);
        uiContainer.alpha = 0;
        otherUIs.SetActive(false);
        mouseRaycaster.enabled = false;
        creditsCanvasGroup.alpha = 0;
        creditsCanvasGroup.gameObject.SetActive(false);
        uiContainer.DOFade(1, 0.5f);
        isMenuOpen = true;
    }

    public void StartGame()
    {
        uiContainer.DOFade(0, 0.5f).OnComplete(() => ui.SetActive(false));
        otherUIs.SetActive(true);
        vcam.Priority = -100;
        mouseRaycaster.enabled = true;

        if (isGameLaunch)
        {
            isGameLaunch = false;
            tutorial.gameObject.SetActive(true);
        }

        isMenuOpen = false;
    }

    public void ToggleCredits(bool value)
    {
        if (value)
        {
            creditsCanvasGroup.gameObject.SetActive(true);
            uiContainer.DOFade(0, 0.5f).OnComplete(() =>
            {
                creditsCanvasGroup.DOFade(1, 0.5f).OnComplete(() => { creditsActive = true; });
            });
            return;
        }

        creditsActive = false;
        creditsCanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            creditsCanvasGroup.gameObject.SetActive(false);
            uiContainer.DOFade(1, 0.5f);
            creditsBody.anchoredPosition = Vector2.zero;
        });
    }

    public void ToggleSettings(bool value)
    {
        uiContainer.DOFade(value ? 0 : 1, 0.5f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // private IEnumerator RollCredits()
    // {
    //     while (true)
    //     {
    //         creditsBody.anchoredPosition += creditsSpeed * Time.deltaTime * Vector2.up;
    //         yield return null;
    //     }
    // }

    private bool creditsActive = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (!isMenuOpen)
                ShowMainMenu();

        if (!creditsActive) return;

        creditsBody.anchoredPosition += creditsSpeed * Time.deltaTime * Vector2.up;

        var scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            var manualSpeed = 50f;
            creditsBody.anchoredPosition += scroll * manualSpeed * Vector2.up;
        }
    }
}