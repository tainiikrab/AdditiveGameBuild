using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animation loadingAnimation;
    [SerializeField] private Slider progressBar;

    // [SerializeField] private ReviewUI reviewUI;
    public static SceneController instance { get; private set; }
    public static bool isMinigameFinished = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public static void OpenScene(Scenes scene)
    {
        if (instance != null)
            instance.StartCoroutine(instance.LoadSceneAsync(scene));
        else
            SceneManager.LoadScene((int)scene);
    }

    private IEnumerator LoadSceneAsync(Scenes scene)
    {
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

        if (isMinigameFinished == true)
        {
            isMinigameFinished = false;
            // Debug.Log("Quality:" + OrderManager.currentOrderQuality.totalQuality.ToString());
            // if (reviewUI != null)
            // {
            //     reviewUI.gameObject.SetActive(true);
            //     reviewUI.Initialize(OrderManager.currentOrder);
            // }
        }
    }
}


public enum Scenes
{
    MainScene,
    PostProcessMinigame
}