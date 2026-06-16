using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnXPChanged;
    public event Action<int> OnLevelChanged;
    public event Action<PickaxeData> OnPickaxeChanged;
    public event Action<AutoCollectorData> OnAutoCollectorChanged;

    [SerializeField] private List<PickaxeData> pickaxes;

    [SerializeField] private List<AutoCollectorData> autoCollectors;
    [SerializeField] private int baseXPPerLevel = 100;
    [SerializeField] private float xpScalingFactor = 1.5f;
    public int Coins { get; private set; }
    public int TotalXP { get; private set; }
    public int PlayerLevel { get; private set; } = 1;
    public int CurrentXP { get; private set; }

    public PickaxeData ActivePickaxe { get; private set; }
    public AutoCollectorData ActiveAutoCollector { get; private set; }

    private int _pickaxeTierIndex = 0;
    private int _autoCollectorTierIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ActivePickaxe = pickaxes[0];
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }

    public bool SpendCoins(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        OnCoinsChanged?.Invoke(Coins);
        return true;
    }

    public void AddXP(int amount)
    {
        TotalXP += amount;
        CurrentXP += amount;
        OnXPChanged?.Invoke(CurrentXP);

        while (CurrentXP >= XPForNextLevel())
        {
            CurrentXP -= XPForNextLevel();
            PlayerLevel++;
            OnLevelChanged?.Invoke(PlayerLevel);
        }
    }

    public int XPForNextLevel()
    {
        return Mathf.RoundToInt(baseXPPerLevel * Mathf.Pow(xpScalingFactor, PlayerLevel - 1));
    }

    public PickaxeData NextPickaxe()
    {
        int next = _pickaxeTierIndex + 1;
        return next < pickaxes.Count ? pickaxes[next] : null;
    }

    public bool TryUpgradePickaxe()
    {
        PickaxeData next = NextPickaxe();
        if (next == null) return false;
        if (!SpendCoins(next.cost)) return false;

        _pickaxeTierIndex++;
        ActivePickaxe = pickaxes[_pickaxeTierIndex];
        OnPickaxeChanged?.Invoke(ActivePickaxe);
        return true;
    }

    public AutoCollectorData NextAutoCollector()
    {
        int next = _autoCollectorTierIndex + 1;
        return next < autoCollectors.Count ? autoCollectors[next] : null;
    }

    public bool TryUpgradeAutoCollector()
    {
        AutoCollectorData next = NextAutoCollector();
        if (next == null) return false;
        if (!SpendCoins(next.cost)) return false;

        _autoCollectorTierIndex++;
        ActiveAutoCollector = autoCollectors[_autoCollectorTierIndex];
        OnAutoCollectorChanged?.Invoke(ActiveAutoCollector);
        return true;
    }

    public List<PickaxeData> AllPickaxes => pickaxes;
    public List<AutoCollectorData> AllAutoCollectors => autoCollectors;
}
