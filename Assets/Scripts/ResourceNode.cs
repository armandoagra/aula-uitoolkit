using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceNode : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Renderer _renderer;
    public event Action<ResourceNode, int> OnCollected;
    public event Action<int, int> OnProgressChanged;
    public event Action<ResourceNode> OnDespawned;

    public ResourceData Data { get; private set; }
    public bool IsCollected { get; private set; }
    public int ClicksLeft { get; private set; }
    public int TotalClicks { get; private set; }

    public List<ResourceSO> Resources;

    private bool _initialised;

    private void Update()
    {
        if (!_initialised || IsCollected) return;
    }

    public void Initialise(ResourceData data)
    {
        Data = data;

        int clicks = Mathf.Max(1, data.baseClicksRequired);
        TotalClicks = clicks;
        ClicksLeft = clicks;

        _initialised = true;

        _meshFilter.mesh = data.MeshOptions[UnityEngine.Random.Range(0, data.MeshOptions.Count)];
        _renderer.material = data.Material;
        var boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = _meshFilter.mesh.bounds.center;
        boxCollider.size = _meshFilter.mesh.bounds.size;
        OnProgressChanged?.Invoke(ClicksLeft, TotalClicks);
    }

    public void OnPlayerClick()
    {
        Debug.Log("Clicked");
        if (!_initialised || IsCollected) return;
        Debug.Log("yes");
        PickaxeData pickaxe = GameManager.Instance.ActivePickaxe;
        ApplyClicks(pickaxe != null ? pickaxe.clickPower : 1);
    }

    public void ApplyAutoClick(int power)
    {
        if (!_initialised || IsCollected) return;
        ApplyClicks(power);
    }

    private void ApplyClicks(int power)
    {
        ClicksLeft = Mathf.Max(0, ClicksLeft - power);
        OnProgressChanged?.Invoke(ClicksLeft, TotalClicks);

        if (ClicksLeft <= 0)
            Collect();
    }

    private void Collect()
    {
        if (IsCollected) return;
        IsCollected = true;

        PickaxeData pickaxe = GameManager.Instance.ActivePickaxe;
        float mult = pickaxe != null ? pickaxe.rewardMultiplier : 1f;
        int coins = Mathf.RoundToInt(Data.baseReward * mult);

        GameManager.Instance.AddCoins(coins);
        GameManager.Instance.AddXP(Data.xpReward);
        Resources[Data.resourceType].ResourceAmount++;
        OnCollected?.Invoke(this, coins);
        OnDespawned?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        OnPlayerClick();
    }

}
