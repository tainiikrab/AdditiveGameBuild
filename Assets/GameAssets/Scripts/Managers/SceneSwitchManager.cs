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

    public static SceneName currentScene { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
        currentScene = SceneName.MainScene;
    }

    public static void ReloadScene()
    {
        OpenScene(currentScene);
    }

    public static void OpenScene(SceneName sceneName, bool instant = false)
    {
        currentScene = sceneName;
        if (Instance == null)
        {
            Debug.Log("No instance for sceneswitchmanager");
            SceneManager.LoadScene((int)sceneName);
            return;
        }

        if (Instance.reviewUI.canvasGroup != null) Instance.reviewUI.canvasGroup.alpha = 0;
        Instance.reviewUI.gameObject.SetActive(false);

        if (instant)
        {
            SceneManager.LoadScene((int)sceneName);
            return;
        }

        Instance.StartCoroutine(Instance.LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(SceneName sceneName)
    {
        Debug.Log("Loading async");
        loadingScreen.SetActive(true);

        if (loadingAnimation != null)
            loadingAnimation.Play();

        var operation = SceneManager.LoadSceneAsync((int)sceneName);
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

        //if (isMinigameFinished) OpenReviewUI();
    }

# if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) OpenReviewUI();
    }
#endif
    private void OpenReviewUI()
    {
        isMinigameFinished = false;
        if (reviewUI != null)
        {
            reviewUI.gameObject.SetActive(true);
            reviewUI.Initialize();
        }
    }
}


public enum SceneName
{
    MainScene,
    PostProcessMinigame,
    ScannerMinigame,
    customizationMiniGame
}