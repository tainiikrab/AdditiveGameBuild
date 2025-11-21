using UnityEngine;

public class ModelSpawnder : MonoBehaviour
{
    public GameObject modelParent;
    public GameObject modelPrefab;

    public Material paintableMaterial;

    private GameObject spawnedModel;

    private void Start()
    {
        if (OrderManager.orderData != null)
        {
            modelPrefab = OrderManager.orderData.config.mesh;

            if (modelPrefab == null)
            {
                Debug.LogError("Model prefab is null!");
                return;
            }

            spawnedModel = Instantiate(modelPrefab, modelParent.transform);

            var renderers = spawnedModel.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var mats = new Material[renderer.materials.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = paintableMaterial;
                }
                renderer.materials = mats;
            }

            if (spawnedModel.GetComponent<MeshCollider>() == null)
            {
                var meshFilter = spawnedModel.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    var collider = spawnedModel.AddComponent<MeshCollider>();
                    collider.sharedMesh = meshFilter.sharedMesh;
                }
                else
                {
                    var meshFilters = spawnedModel.GetComponentsInChildren<MeshFilter>();
                    foreach (var mf in meshFilters)
                    {
                        if (mf.gameObject.GetComponent<MeshCollider>() == null && mf.sharedMesh != null)
                        {
                            var mc = mf.gameObject.AddComponent<MeshCollider>();
                            mc.sharedMesh = mf.sharedMesh;
                        }
                    }
                }
            }

            if (spawnedModel.GetComponent<PaintableObject>() == null)
            {
                spawnedModel.AddComponent<PaintableObject>();
            }

            if (spawnedModel.GetComponent<ObjectManipulator>() == null)
            {
                spawnedModel.AddComponent<ObjectManipulator>();
            }
        }
        else
        {
            Debug.LogError("OrderManager.orderData is null!");
        }
    }
}
