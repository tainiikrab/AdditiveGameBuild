using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitchManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animation loadingAnimation;
    [SerializeField] private Slider progressBar;

    [SerializeField] private ReviewUI reviewUI;

    // [SerializeField] private ReviewUI reviewUI;
    public static SceneSwitchManager Instance { get; private set; }
    public static bool isMinigameFinished = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public static void OpenScene(Scenes scene)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.LoadSceneAsync(scene));
            return;
        }

        Debug.Log("No instance for sceneswitchmanager");
        SceneManager.LoadScene((int)scene);
    }

    private IEnumerator LoadSceneAsync(Scenes scene)
    {
        Debug.Log("Loading async");
        loadingScreen.SetActive(true);

        if (loadingAnimation != null)
            loadingAnimation.Play();

        var operation = SceneManager.LoadSceneAsync((int)scene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            if (operation.progress >= 0.9f)
                // yield return new WaitForSeconds(0.2f);
                operation.allowSceneActivation = true;

            yield return null;
        }

        loadingScreen.SetActive(false);

        if (isMinigameFinished)
        {
            isMinigameFinished = false;
            Debug.Log("Quality:" + OrderManager.currentOrderQuality.totalQuality.ToString());
            if (reviewUI != null)
            {
                reviewUI.gameObject.SetActive(true);
                reviewUI.Initialize(OrderManager.currentOrder);
            }
        }
    }
}


public enum Scenes
{
    MainScene,
    PostProcessMinigame
}