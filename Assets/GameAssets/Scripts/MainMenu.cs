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
    [SerializeField] private GameObject characterDialog;

    private RectTransform creditsBody;

    private static bool isGameLaunch = true;
    private bool isMenuOpen = false;
    private bool creditsActive = false;

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

        // DOTween animations ignore timeScale
        uiContainer.DOFade(1, 0.5f).SetUpdate(true);

        Time.timeScale = 0; // freeze gameplay
        isMenuOpen = true;
    }

    public void StartGame()
    {
        uiContainer.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => ui.SetActive(false));
        otherUIs.SetActive(true);
        vcam.Priority = -100;
        mouseRaycaster.enabled = true;

        if (isGameLaunch)
        {
            isGameLaunch = false;
            tutorial.gameObject.SetActive(true);
        }

        Time.timeScale = 1; // resume gameplay
        isMenuOpen = false;
    }

    public void ToggleCredits(bool value)
    {
        if (value)
        {
            creditsCanvasGroup.gameObject.SetActive(true);
            uiContainer.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() =>
            {
                creditsCanvasGroup.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => { creditsActive = true; });
            });
            return;
        }

        creditsActive = false;
        creditsCanvasGroup.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            creditsCanvasGroup.gameObject.SetActive(false);
            uiContainer.DOFade(1, 0.5f).SetUpdate(true);
            creditsBody.anchoredPosition = Vector2.zero;
        });
    }

    public void ToggleSettings(bool value)
    {
        uiContainer.DOFade(value ? 0 : 1, 0.5f).SetUpdate(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (!isMenuOpen && !characterDialog.activeSelf)
                ShowMainMenu();

        if (!creditsActive) return;

        // auto scroll (unscaled time so it works while paused)
        creditsBody.anchoredPosition += creditsSpeed * Time.unscaledDeltaTime * Vector2.up;

        // mouse wheel scroll
        var scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            var manualSpeed = 50f;
            creditsBody.anchoredPosition += scroll * manualSpeed * Vector2.up;
        }
    }
}