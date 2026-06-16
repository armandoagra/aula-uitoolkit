using UnityEngine;

[CreateAssetMenu(fileName = "NewPickaxe", menuName = "IdleMiner/Pickaxe Data")]
public class PickaxeData : ScriptableObject
{
    public string pickaxeName = "Wooden Pickaxe";
    public Sprite icon;
    public int clickPower = 1;
    public float rewardMultiplier = 1f;
    public int cost = 0;
    public int tier = 0;
}
