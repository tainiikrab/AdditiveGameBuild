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

    [Header("Overhang Settings")] [SerializeField]
    private float overhangAngleDeg = 45f; // printable angle threshold

    [SerializeField] private float minSupportHeight = 0.01f; // avoid micro supports
    [SerializeField] private float placementGridSize = 0.01f;

    private Transform model;
    private Bounds modelBounds;
    private float baseY;
    [SerializeField] private float minSupportSpacing = 0.5f; // world units
    [SerializeField] private float bottomExclusionHeight = 0.005f;


    public static float sandpaperingAmount = 0f;
    public static float removedSupports = 0;

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
        supportScale = 1 / model.transform.localScale.x;

        var renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            var combinedBounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                combinedBounds.Encapsulate(renderers[i].bounds);

            modelBounds = combinedBounds;
            baseY = ComputeModelLowestY(model);

            SpawnSupportsOverhangs();
        }
    }

    private float ComputeModelLowestY(Transform root)
    {
        var lowest = float.PositiveInfinity;
        var meshFilters = root.GetComponentsInChildren<MeshFilter>();
        foreach (var mf in meshFilters)
        {
            var mesh = mf.sharedMesh;
            if (!mesh) continue;
            foreach (var v in mesh.vertices)
            {
                var w = mf.transform.TransformPoint(v);
                if (w.y < lowest) lowest = w.y;
            }
        }

        return float.IsPositiveInfinity(lowest) ? modelBounds.min.y : lowest;
    }

    private void SpawnSupportsOverhangs()
    {
        var meshFilters = model.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0) return;

        var spawned = 0;
        var spawnedPositions = new List<Vector3>();

        while (spawned < supportCount)
        {
            // pick a random mesh
            var mf = meshFilters[Random.Range(0, meshFilters.Length)];
            var mesh = mf.sharedMesh;
            if (!mesh) continue;

            var verts = mesh.vertices;
            var tris = mesh.triangles;

            if (tris.Length < 3)
                continue;

            // Pick a valid triangle index (0, 3, 6, ...)
            var triStart = Random.Range(0, tris.Length / 3) * 3;

            // Safe vertex access USING triangles array
            var p0 = mf.transform.TransformPoint(verts[tris[triStart]]);
            var p1 = mf.transform.TransformPoint(verts[tris[triStart + 1]]);
            var p2 = mf.transform.TransformPoint(verts[tris[triStart + 2]]);

            // Random point inside this triangle
            var randomPoint = RandomPointInTriangle(p0, p1, p2);

            // Skip points too close to the bottom of the model
            if (randomPoint.y - baseY < bottomExclusionHeight)
                continue;

            // Spacing check
            var tooClose = false;
            foreach (var pos in spawnedPositions)
                if (Vector3.Distance(pos, randomPoint) < minSupportSpacing)
                {
                    tooClose = true;
                    break;
                }

            if (tooClose) continue;

            // Spawn support at that surface point
            SpawnSupportAt(randomPoint, randomPoint);

            spawnedPositions.Add(randomPoint);
            spawned++;
        }
    }


    private Vector3 RandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        var r1 = Random.value;
        var r2 = Random.value;

        // Ensure uniform distribution inside triangle
        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        return a + (b - a) * r1 + (c - a) * r2;
    }

    private void SpawnSupportAt(Vector3 basePos, Vector3 topPos)
    {
        // Just place the prefab at the base position
        var support = Instantiate(supportPrefab, basePos, Quaternion.identity, model);

        // Do not modify its scale at all
        // support.transform.localScale = Vector3.one * supportScale; // <-- remove this line

        // If you want the support to visually connect to the overhang,
        // you can instead stretch it via a script on SupportModel itself,
        // or design the prefab so it already has the right proportions.
    }


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
        // SceneSwitchManager.isMinigameFinished = true;
        OrderManager.orderData.quality.supports = removedSupports / supportCount * 100;
        OrderManager.orderData.quality.sandpapering =
            100 * Mathf.Abs(SandpaperTool.requiredSmoothness - SandpaperTool.smoothnessDone);
        SceneSwitchManager.OpenScene(SceneName.MainScene);
    }
}