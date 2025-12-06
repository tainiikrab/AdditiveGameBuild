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

    public static float sandpaperingAmount = 0f;
    public static int removedSupports = 0;

    private void Awak()
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
        var cosThreshold = Mathf.Cos(overhangAngleDeg * Mathf.Deg2Rad);
        var spawnedPositions = new List<Vector3>();
        var spawned = 0;

        var meshFilters = model.GetComponentsInChildren<MeshFilter>();
        foreach (var mf in meshFilters)
        {
            if (spawned >= supportCount) break;
            var mesh = mf.sharedMesh;
            if (!mesh) continue;

            var verts = mesh.vertices;
            var normals = mesh.normals;
            var tris = mesh.triangles;
            if (normals == null || normals.Length == 0)
            {
                mesh.RecalculateNormals();
                normals = mesh.normals;
            }

            for (var i = 0; i < tris.Length; i += 3)
            {
                if (spawned >= supportCount) break;

                int i0 = tris[i], i1 = tris[i + 1], i2 = tris[i + 2];
                var p0 = mf.transform.TransformPoint(verts[i0]);
                var p1 = mf.transform.TransformPoint(verts[i1]);
                var p2 = mf.transform.TransformPoint(verts[i2]);
                var c = (p0 + p1 + p2) / 3f;

                var nLocal = (normals[i0] + normals[i1] + normals[i2]) / 3f;
                var n = mf.transform.TransformDirection(nLocal).normalized;

                var dotUp = Vector3.Dot(n, Vector3.up);
                if (dotUp > cosThreshold) continue;
                if (dotUp < -0.5f) continue;

                var rayOrigin = c + Vector3.up * 0.001f;
                Vector3 basePos;
                // if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity))
                //     basePos = hit.point;
                // else
                //     basePos = new Vector3(c.x, baseY, c.z);
                if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity))
                    // If we hit something below, use that point
                    basePos = hit.point;
                else
                    // Nothing below until bed → skip (don’t spawn at bottom)
                    continue;

                // Check spacing
                var tooClose = false;
                foreach (var pos in spawnedPositions)
                    if (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(basePos.x, basePos.z)) <
                        minSupportSpacing)
                    {
                        tooClose = true;
                        break;
                    }

                if (tooClose) continue;

                SpawnSupportAt(basePos, c);
                spawnedPositions.Add(basePos);
                spawned++;
            }
        }
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
        SceneSwitchManager.isMinigameFinished = true;
        OrderManager.orderData.quality.supports = 100 - (supportCount - removedSupports) * 10;
        OrderManager.orderData.quality.sandpapering = sandpaperingAmount * 100;
        SceneSwitchManager.OpenScene(SceneName.MainScene);
    }
}