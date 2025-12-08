using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class PostprocessMinigame : MonoBehaviour
{
    [SerializeField] private Button finishButton;
    [SerializeField] private ModelRotator modelRotator;

    [Header("Support Settings")] [SerializeField]
    private SupportModel supportPrefab;

    [SerializeField] private int supportCount = 20;
    [SerializeField] private float supportScale = 0.2f;

    [Header("Raycast Spawn Settings")] [SerializeField]
    private float minSupportSpacing = 0.5f;

    private Transform model;
    private Bounds modelBounds;
    private float baseY;
    [SerializeField] private float bottomExclusionHeight = 0.005f;

    public static float sandpaperingAmount = 0f;
    public static float removedSupports = 0;

    [SerializeField] private string MODEL_LAYER_NAME = "ModelSurface";

    private void Awake()
    {
        finishButton.onClick.AddListener(FinishGame);
        sandpaperingAmount = 0f;
        removedSupports = 0;

        AudioManager.Instance.PlayMusic(MusicType.BackgroundMusic);
    }

    private void Start()
    {
        model = modelRotator.model;
        supportScale = 1 / Mathf.Max(model.transform.localScale.x, 0.0001f);

        SetupBounds();
        SetupColliders();
        SetupLayer();

        SpawnSupportsViaRaycasts();
    }

    // -------------------------------------------------------------------------
    // BOUNDS
    // -------------------------------------------------------------------------
    private void SetupBounds()
    {
        var renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        var combinedBounds = renderers[0].bounds;
        for (var i = 1; i < renderers.Length; i++)
            combinedBounds.Encapsulate(renderers[i].bounds);

        modelBounds = combinedBounds;
        baseY = modelBounds.min.y;
    }

    // -------------------------------------------------------------------------
    // COLLIDER SETUP
    // -------------------------------------------------------------------------
    private void SetupColliders()
    {
        // Ensure every mesh under the model has a MeshCollider
        foreach (var mf in model.GetComponentsInChildren<MeshFilter>())
        {
            var mc = mf.GetComponent<MeshCollider>();
            if (mc == null)
            {
                mc = mf.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
                mc.convex = false; // must be non-convex to get full geometry hits
            }
        }
    }

    // -------------------------------------------------------------------------
    // LAYER SETUP
    // -------------------------------------------------------------------------
    private void SetupLayer()
    {
        var layer = LayerMask.NameToLayer(MODEL_LAYER_NAME);
        if (layer == -1)
        {
            Debug.LogWarning("Layer 'ModelSurface' does not exist! Create it in Project Settings → Tags & Layers.");
            layer = 0;
        }

        foreach (var t in model.GetComponentsInChildren<Transform>())
            t.gameObject.layer = layer;
    }

    // -------------------------------------------------------------------------
    // SUPPORT SPAWNING (NEW RAYCAST LOGIC)
    // -------------------------------------------------------------------------
    private void SpawnSupportsViaRaycasts()
    {
        var layerMask = 1 << LayerMask.NameToLayer(MODEL_LAYER_NAME);
        var rayDistance = modelBounds.extents.magnitude * 4f;

        List<Vector3> spawnedPositions = new();
        var spawned = 0;
        var attempts = 0;
        var maxAttempts = supportCount * 40;

        while (spawned < supportCount && attempts < maxAttempts)
        {
            attempts++;

            // origin on sphere
            var origin =
                modelBounds.center + Random.onUnitSphere * modelBounds.extents.magnitude * 2f;
            // aim at random point inside bounds
            var randomTarget = new Vector3(
                Random.Range(modelBounds.min.x, modelBounds.max.x),
                Random.Range(modelBounds.min.y, modelBounds.max.y),
                Random.Range(modelBounds.min.z, modelBounds.max.z)
            );

            var dir = (randomTarget - origin).normalized;

            if (Physics.Raycast(origin, dir, out var hit, rayDistance, layerMask))
            {
                // bottom exclusion
                if (hit.point.y - baseY < bottomExclusionHeight)
                    continue;

                // spacing check
                var tooClose = false;
                foreach (var pos in spawnedPositions)
                    if (Vector3.Distance(pos, hit.point) < minSupportSpacing)
                    {
                        tooClose = true;
                        break;
                    }

                if (tooClose) continue;

                // spawn support
                SpawnSupportAt(hit.point);

                spawnedPositions.Add(hit.point);
                spawned++;
            }
        }

        Debug.Log($"Spawned {spawned}/{supportCount} supports.");
    }

    // -------------------------------------------------------------------------
    // SUPPORT INSTANTIATION (unchanged)
    // -------------------------------------------------------------------------
    private void SpawnSupportAt(Vector3 pos)
    {
        Instantiate(supportPrefab, pos, Quaternion.identity, model);
    }

    // -------------------------------------------------------------------------
    private bool IsTransformInHierarchy(Transform t, Transform root)
    {
        while (t != null)
        {
            if (t == root) return true;
            t = t.parent;
        }

        return false;
    }

    private void FinishGame()
    {
        OrderManager.orderData.quality.supports = removedSupports / supportCount * 100f;
        OrderManager.orderData.quality.sandpapering =
            100 * Mathf.Abs(SandpaperTool.requiredSmoothness - SandpaperTool.smoothnessDone);

        SceneSwitchManager.OpenScene(SceneName.MainScene);
    }
}