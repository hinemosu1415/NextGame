using System.Collections.Generic;
using UnityEngine;

public class StructureMenuSlotManager : MonoBehaviour
{
    [SerializeField] private GameSaveData _gameData;
    [SerializeField] private StructureDatabase _structureDatabase;
    [SerializeField] private Transform _selectedStructureContent;
    [SerializeField] private Transform _structureInventoryContent;
    [SerializeField] private StructureMenuSlot _structureSlotPre;

    private StructureMenuSlot _targetSlot;

    private readonly List<StructureMenuSlot> _selectedStructureList = new();
    private readonly List<StructureMenuSlot> _inventoryStructureList = new();

    private void Start()
    {
        CreateSelectedSlots();
        InitializeTargetSlot();
        CreateInventorySlots();
        UpdateInventoryFrames();
    }

    private void OnDestroy()
    {
        foreach (var slot in _selectedStructureList)
        {
            slot.OnSelect -= SelectTargetSlot;
        }

        foreach (var slot in _inventoryStructureList)
        {
            slot.OnSelect -= SelectStructure;
        }
    }

    private void CreateSelectedSlots()
    {
        foreach (var data in _gameData.SelectedStructures)
        {
            StructureMenuSlot slot = Instantiate(
                _structureSlotPre,
                _selectedStructureContent
            );

            slot.UpdateStructureData(data);
            slot.OnSelect += SelectTargetSlot;

            _selectedStructureList.Add(slot);
        }
    }

    private void InitializeTargetSlot()
    {
        if (_selectedStructureList.Count == 0)
        {
            return;
        }

        _targetSlot = _selectedStructureList[^1];
        _targetSlot.ShowFrame();
    }

    private void CreateInventorySlots()
    {
        foreach (var structureData in _structureDatabase.Database)
        {
            StructureMenuSlot slot = Instantiate(
                _structureSlotPre,
                _structureInventoryContent
            );

            slot.UpdateStructureData(structureData);
            slot.OnSelect += SelectStructure;

            _inventoryStructureList.Add(slot);
        }
    }

    private void SelectTargetSlot(StructureMenuSlot selectedSlot)
    {
        _targetSlot = selectedSlot;

        foreach (var slot in _selectedStructureList)
        {
            slot.HideFrame();
        }

        _targetSlot.ShowFrame();
    }

    private void SelectStructure(StructureMenuSlot selectedSlot)
    {
        if (_targetSlot == null)
        {
            return;
        }

        // 既に選択されている建造物なら何もしない
        if (IsSelected(selectedSlot.StructureData))
        {
            return;
        }

        int targetIndex = _selectedStructureList.IndexOf(_targetSlot);

        if (targetIndex < 0)
        {
            return;
        }

        // UIを更新
        _targetSlot.UpdateStructureData(selectedSlot.StructureData);

        // セーブデータを更新
        _gameData.SetSelectedStructure(
            targetIndex,
            selectedSlot.StructureData
        );

        // インベントリ側の選択状態を更新
        UpdateInventoryFrames();
    }

    private bool IsSelected(StructureData structureData)
    {
        foreach (var slot in _selectedStructureList)
        {
            if (slot.StructureData == structureData)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateInventoryFrames()
    {
        foreach (var inventorySlot in _inventoryStructureList)
        {
            if (IsSelected(inventorySlot.StructureData))
            {
                inventorySlot.ShowFrame();
            }
            else
            {
                inventorySlot.HideFrame();
            }
        }
    }
}