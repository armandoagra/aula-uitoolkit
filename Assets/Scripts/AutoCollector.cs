using System.Collections.Generic;
using UnityEngine;

public class AutoCollector : MonoBehaviour
{
    [SerializeField] private ResourceSpawner spawner;

    private readonly Dictionary<ResourceNode, float> _nodeTimers = new Dictionary<ResourceNode, float>();

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnAutoCollectorChanged += OnAutoCollectorChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnAutoCollectorChanged -= OnAutoCollectorChanged;
    }

    private void Update()
    {
        AutoCollectorData collector = GameManager.Instance?.ActiveAutoCollector;
        if (collector == null) return;

        IReadOnlyList<ResourceNode> nodes = spawner.ActiveNodes;

        int targetCount = 0;

        var toRemove = new List<ResourceNode>();
        foreach (var key in _nodeTimers.Keys)
            if (key == null) toRemove.Add(key);
        foreach (var key in toRemove)
            _nodeTimers.Remove(key);

        foreach (ResourceNode node in nodes)
        {
            if (node == null || node.IsCollected) continue;
            if (targetCount >= collector.simultaneousTargets) break;

            targetCount++;

            if (!_nodeTimers.ContainsKey(node))
                _nodeTimers[node] = GetInterval(node, collector);

            _nodeTimers[node] -= Time.deltaTime;

            if (_nodeTimers[node] <= 0f)
            {
                node.ApplyAutoClick(collector.clickPowerPerTick);
                _nodeTimers[node] = GetInterval(node, collector);
            }
        }
    }

    private float GetInterval(ResourceNode node, AutoCollectorData collector)
    {
        float baseInterval = node.Data != null ? node.Data.autoCollectInterval : 2f;
        return baseInterval / Mathf.Max(0.01f, collector.speedMultiplier);
    }

    private void OnAutoCollectorChanged(AutoCollectorData newCollector)
    {
        var keys = new List<ResourceNode>(_nodeTimers.Keys);
        foreach (var key in keys)
            _nodeTimers[key] = GetInterval(key, newCollector);
    }
}
