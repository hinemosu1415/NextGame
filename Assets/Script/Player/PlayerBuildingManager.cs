using System;
using System.Collections.Generic;
using UnityEngine;

public class StructureEntry
{
    public readonly StructureData StructureData;
    public bool CanAfford { get; private set; }
    public bool IsAvailable => CanAfford;

    public event Action<bool> OnIsAvailableChanged;
    public StructureEntry(StructureData structureData)
    {
        StructureData = structureData;
    }

    public void UpdateHasCost(int amount)
    {
        CanAfford = amount >= StructureData.Cost;
        OnIsAvailableChanged?.Invoke(IsAvailable);
    }
}

[RequireComponent(typeof(CurrencyWallet))]
public class PlayerBuildingManager : MonoBehaviour
{
    [SerializeField] private StructureData[] _structures;
    [SerializeField] private StructurePlacementController _structurePlacement;

    private CurrencyWallet _currencyWallet;
    private int _selectedStructureIndex = 0;
    public bool CanBuildSelectedStructure => _structurePlacement.CurrentBuildCheck.CanBuild;
    public event Action<int> OnSelectedStructureChanged;
    public List<StructureEntry> Entries { get; private set; } = new List<StructureEntry>();

    private void Awake()
    {
        _currencyWallet = GetComponent<CurrencyWallet>();
        foreach (var structure in _structures)
        {
            StructureEntry entry = new StructureEntry(structure);
            entry.UpdateHasCost(_currencyWallet.GetCurrencyAmount(CurrencyData.CurrencyType.Coin));
            Entries.Add(entry);
        }
    }

    private void Start()
    {
        SelectStructure(0); //最初の建造物を選択しておく        
        ExitBuildingMode();
        _currencyWallet.OnCurrencyChanged += OnCoinChanged;
    }

    public void OnDestroy()
    {
        _currencyWallet.OnCurrencyChanged -= OnCoinChanged;
    }

    public void EnterBuildingMode()
    {
        _structurePlacement.gameObject.SetActive(true);
    }

    public void ExitBuildingMode()
    {
        _structurePlacement.gameObject.SetActive(false);
    }

    public void SelectStructure(int index)
    {
        if (index < 0 || index >= Entries.Count) return;

        _selectedStructureIndex = index;
        OnSelectedStructureChanged?.Invoke(index);
        _structurePlacement.SetStructureEntry(Entries[_selectedStructureIndex]);
    }

    private void OnCoinChanged(CurrencyData.CurrencyType type, int amount)
    {
        if (type != CurrencyData.CurrencyType.Coin) return;

        foreach (var e in Entries)
            e.UpdateHasCost(amount);
    }

    public void TryPlaceStructure()
    {
        if (_structurePlacement.TryPlaceStructure(gameObject))
        {
            _currencyWallet.TryConsumeCurrency(CurrencyData.CurrencyType.Coin, Entries[_selectedStructureIndex].StructureData.Cost);
        }
    }
}