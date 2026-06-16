using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "IdleMiner/Resource Data")]
public class ResourceData : ScriptableObject
{
    public string resourceName = "Stone";
    public int resourceType = 0;
    public Sprite icon;
    public List<Mesh> MeshOptions;
    public Material Material;
    public int baseClicksRequired = 1;
    public int baseReward = 1;
    public int xpReward = 1;
    public int minPlayerLevel = 1;
    public float spawnWeight = 10f;
    public float autoCollectInterval = 2f;
}
