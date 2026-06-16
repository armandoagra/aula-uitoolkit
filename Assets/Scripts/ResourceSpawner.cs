using System.Collections.Generic;
using UnityEngine;


public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private ResourceNode resourceNodePrefab;
    [SerializeField] private List<ResourceData> allResources;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-5f, -3f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2( 5f,  3f);
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxActiveNodes = 10;
    [SerializeField] private float minSpawnDistance = 0.8f;
    [SerializeField] private int maxPositionRetries = 10;

    private readonly List<ResourceNode> _activeNodes = new List<ResourceNode>();
    private float _spawnTimer;

    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= spawnInterval)
        {
            _spawnTimer = 0f;
            TrySpawn();
        }
    }

    public IReadOnlyList<ResourceNode> ActiveNodes => _activeNodes;

    private void TrySpawn()
    {
        CleanupNullNodes();

        if (_activeNodes.Count >= maxActiveNodes) return;

        ResourceData chosen = PickWeightedRandom();
        if (chosen == null) return;

        Vector2? position = FindValidPosition();
        if (position == null) return;

        ResourceNode node = Instantiate(resourceNodePrefab, new Vector3(position.Value.x, 0f, position.Value.y), Quaternion.identity);
        node.Initialise(chosen);
        node.OnDespawned += HandleNodeDespawned;
        _activeNodes.Add(node);
    }

    private ResourceData PickWeightedRandom()
    {
        int playerLevel = GameManager.Instance != null ? GameManager.Instance.PlayerLevel : 1;

        var eligible = new List<ResourceData>();
        float totalWeight = 0f;

        foreach (ResourceData r in allResources)
        {
            if (r == null) continue;
            if (r.minPlayerLevel > playerLevel) continue;
            eligible.Add(r);
            totalWeight += r.spawnWeight;
        }

        if (eligible.Count == 0) return null;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (ResourceData r in eligible)
        {
            cumulative += r.spawnWeight;
            if (roll <= cumulative)
                return r;
        }

        return eligible[eligible.Count - 1];
    }

    private Vector2? FindValidPosition()
    {
        for (int attempt = 0; attempt < maxPositionRetries; attempt++)
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            Vector2 candidate = new Vector2(x, y);

            bool tooClose = false;
            foreach (ResourceNode existing in _activeNodes)
            {
                if (existing == null) continue;
                if (Vector2.Distance(candidate, existing.transform.position) < minSpawnDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return candidate;
        }

        return null;
    }

    private void HandleNodeDespawned(ResourceNode node)
    {
        _activeNodes.Remove(node);
    }

    private void CleanupNullNodes()
    {
        _activeNodes.RemoveAll(n => n == null);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(
            (spawnAreaMin.x + spawnAreaMax.x) / 2f,
            0.3f,
            (spawnAreaMin.y + spawnAreaMax.y) / 2f);
        Vector3 size = new Vector3(
            spawnAreaMax.x - spawnAreaMin.x,
            1f,
            spawnAreaMax.y - spawnAreaMin.y);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
