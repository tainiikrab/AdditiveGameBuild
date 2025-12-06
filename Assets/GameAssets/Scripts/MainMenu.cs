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

    private RectTransform creditsBody;

    private void Awake()
    {
        otherUIs.SetActive(false);
        // settingsPanel?.gameObject.SetActive(false);
        mouseRaycaster.enabled = false;
        creditsCanvasGroup.alpha = 0;
        creditsCanvasGroup.gameObject.SetActive(false);
        uiContainer.alpha = 1;
        ui.SetActive(true);
        creditsBody = creditsCanvasGroup.transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void StartGame()
    {
        uiContainer.DOFade(0, 0.5f).OnComplete(() => ui.SetActive(false));
        otherUIs.SetActive(true);
        vcam.Priority = -100;
        mouseRaycaster.enabled = true;
    }

    public void ToggleCredits(bool value)
    {
        if (value)
        {
            StartCoroutine(RollCredits());
            creditsCanvasGroup.gameObject.SetActive(true);
            uiContainer.DOFade(0, 0.5f).OnComplete(() => creditsCanvasGroup.DOFade(1, 0.5f));
            return;
        }

        StopCoroutine(RollCredits());
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

    private IEnumerator RollCredits()
    {
        while (true)
        {
            creditsBody.anchoredPosition += Vector2.up * creditsSpeed;
            yield return null;
        }
    }
}