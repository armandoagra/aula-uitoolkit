using System.Collections.Generic;
using Mono.Cecil;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Scriptable Objects/PlayerDataSO")]
public class ResourceSO : ScriptableObject
{
    public int ResourceAmount;

    [CreateProperty]
    public DisplayStyle IsVisible
    {
        get
        {
            if (ResourceAmount > 0) return DisplayStyle.Flex;
            else return DisplayStyle.None;
        }
    }
}
