using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Unity.Cinemachine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Light workplaceLight;
    [SerializeField] private CinemachineCamera workplaceCamera;
    [SerializeField] private CanvasGroup workplaceTutorialUI;
    public static Tutorial instance { get; private set; }

    private float workplaceLightIntensity;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (!workplaceLight.gameObject.activeSelf) workplaceLight.gameObject.SetActive(true);
        workplaceLightIntensity = workplaceLight.intensity;
        workplaceLight.intensity = 0; // start dark
    }

    public void ShowWorkplaceTutorial()
    {
        workplaceLight.DOIntensity(workplaceLightIntensity, 1f);
        workplaceCamera.Priority = 100;
        workplaceTutorialUI.gameObject.SetActive(true);
        workplaceTutorialUI.DOFade(1, 1f);
    }

    public void HideWorkplaceTutorial()
    {
        workplaceLight.DOIntensity(0, 1f);
        workplaceCamera.Priority = -100;
        workplaceTutorialUI.DOFade(0, 1f).OnComplete(() => workplaceTutorialUI.gameObject.SetActive(false));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) ShowWorkplaceTutorial();
        if (Input.GetKeyDown(KeyCode.Y)) HideWorkplaceTutorial();
    }
}