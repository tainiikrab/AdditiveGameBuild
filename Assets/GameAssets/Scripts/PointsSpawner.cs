using UnityEngine;

/// <summary>
/// Этот скрипт надо поправить.
/// </summary>
public class PointsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Transform spawnArea;
    
    private float minSpawnInterval = 5f;
    private float maxSpawnInterval = 15f;

    private bool isActive;

    private void OnEnable()
    {
        isActive = true;
    }

    private void OnDisable()
    {
        isActive = false;
    }

    private void Update()
    {
        if (!isActive) return;

        if (Random.Range(0f, 1f) < Time.deltaTime / GetRandomInterval())
        {
            Vector3 spawnPosition = new(
                Random.Range(-spawnArea.localScale.x / 2, spawnArea.localScale.x / 2),
                Random.Range(-spawnArea.localScale.y / 2, spawnArea.localScale.y / 2),
                Random.Range(-spawnArea.localScale.z / 2, spawnArea.localScale.z / 2)
            );

            Spawn(pointPrefab, transform.transform.TransformPoint(spawnPosition));
        }
    }

    private void Spawn(GameObject pointPrefab, Vector3 position)
    {
        Instantiate(pointPrefab, position, Quaternion.identity);
    }

    private float GetRandomInterval()
    {
        return Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
