using System;
using UnityEngine;

/// <summary>
/// Pure-logic upgrade shop. Contains no UI references.
/// Call TryBuyNextPickaxe() / TryBuyNextAutoCollector() from UI scripts
/// or from debug inspector buttons.
///
/// Subscribe to the result events to react in UI or other systems.
/// </summary>
public class UpgradeShop : MonoBehaviour
{
    public static UpgradeShop Instance { get; private set; }

    // ── Events ────────────────────────────────────────────────────────────────
    /// <summary>Fired on a successful pickaxe purchase. Carries the new pickaxe.</summary>
    public event Action<PickaxeData>        OnPickaxePurchased;

    /// <summary>Fired on a successful auto-collector purchase/upgrade.</summary>
    public event Action<AutoCollectorData>  OnAutoCollectorPurchased;

    /// <summary>Fired when a purchase fails. Carries a human-readable reason.</summary>
    public event Action<string>             OnPurchaseFailed;

    // ── Unity ─────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Attempts to buy the next pickaxe tier.
    /// Fires OnPickaxePurchased or OnPurchaseFailed.
    /// </summary>
    public void TryBuyNextPickaxe()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        PickaxeData next = gm.NextPickaxe();
        if (next == null)
        {
            OnPurchaseFailed?.Invoke("Already at max pickaxe tier.");
            return;
        }

        if (gm.Coins < next.cost)
        {
            OnPurchaseFailed?.Invoke($"Need {next.cost} coins (have {gm.Coins}).");
            return;
        }

        if (gm.TryUpgradePickaxe())
            OnPickaxePurchased?.Invoke(gm.ActivePickaxe);
    }

    /// <summary>
    /// Attempts to buy or upgrade the auto-collector.
    /// Fires OnAutoCollectorPurchased or OnPurchaseFailed.
    /// </summary>
    public void TryBuyNextAutoCollector()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        AutoCollectorData next = gm.NextAutoCollector();
        if (next == null)
        {
            OnPurchaseFailed?.Invoke("Already at max auto-collector tier.");
            return;
        }

        if (gm.Coins < next.cost)
        {
            OnPurchaseFailed?.Invoke($"Need {next.cost} coins (have {gm.Coins}).");
            return;
        }

        if (gm.TryUpgradeAutoCollector())
            OnAutoCollectorPurchased?.Invoke(gm.ActiveAutoCollector);
    }

    // ── Convenience info queries (safe to call from UI at any time) ───────────

    /// <summary>Returns cost and name of the next pickaxe, or null if maxed.</summary>
    public (string name, int cost)? NextPickaxeInfo()
    {
        PickaxeData next = GameManager.Instance?.NextPickaxe();
        return next != null ? (next.pickaxeName, next.cost) : ((string, int)?)null;
    }

    /// <summary>Returns cost and name of the next auto-collector, or null if maxed.</summary>
    public (string name, int cost)? NextAutoCollectorInfo()
    {
        AutoCollectorData next = GameManager.Instance?.NextAutoCollector();
        return next != null ? (next.collectorName, next.cost) : ((string, int)?)null;
    }
}
