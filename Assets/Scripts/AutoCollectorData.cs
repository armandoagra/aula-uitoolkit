using UnityEngine;

[CreateAssetMenu(fileName = "NewAutoCollector", menuName = "IdleMiner/Auto Collector Data")]
public class AutoCollectorData : ScriptableObject
{
    public string collectorName = "Basic Auto-Miner";
    public Sprite icon;
    public int simultaneousTargets = 1;
    public float speedMultiplier = 1f;
    public int clickPowerPerTick = 1;
    public int cost = 50;
    public int tier = 1;
}
