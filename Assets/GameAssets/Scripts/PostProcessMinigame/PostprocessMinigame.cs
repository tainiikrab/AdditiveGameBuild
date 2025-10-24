using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PostprocessMinigame : MonoBehaviour
{
    [SerializeField] private Button finishButton;
    [SerializeField] private ModelRotator modelRotator;

    [Header("Support Settings")] [SerializeField]
    private SupportModel supportPrefab;

    [SerializeField] private int supportCount = 20;
    [SerializeField] private float supportScale = 0.2f;

    private Transform model;
    private Bounds modelBounds;

    public static float sandpaperingAmount = 0f;
    public static int removedSupports = 0;


    private void Awake()
    {
        finishButton.onClick.AddListener(FinishGame);
        sandpaperingAmount = 0f;
        removedSupports = 0;
    }

    private void Start()
    {
        model = modelRotator.model;

        supportScale = 1 / model.transform.localScale.x;
        var renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            var combinedBounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                combinedBounds.Encapsulate(renderers[i].bounds);

            modelBounds = combinedBounds;
            SpawnSupports();
        }
    }

    private void SpawnSupports()
    {
        var spawned = 0;
        var attempts = 0;

        while (spawned < supportCount && attempts < supportCount * 10)
        {
            attempts++;

            var x = Random.Range(modelBounds.min.x, modelBounds.max.x);
            var z = Random.Range(modelBounds.min.z, modelBounds.max.z);
            var rayOrigin = new Vector3(x, modelBounds.max.y + 1f, z);

            if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, modelBounds.size.y + 2f))
            {
                var pos = new Vector3(x, modelBounds.min.y, z);

                var support = Instantiate(supportPrefab, pos, Quaternion.identity, model);
                support.transform.localScale = Vector3.one * supportScale;

                spawned++;
            }
        }
    }

    private void FinishGame()
    {
        SceneSwitchManager.isMinigameFinished = true;

        OrderManager.currentOrderQuality.supports = 100 - (supportCount - removedSupports) * 10;
        OrderManager.currentOrderQuality.sandpapering = sandpaperingAmount * 100;

        SceneSwitchManager.OpenScene(Scenes.MainScene);
    }
}