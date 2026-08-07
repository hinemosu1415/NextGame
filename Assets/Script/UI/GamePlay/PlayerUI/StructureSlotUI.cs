using System.Collections.Generic;
using UnityEngine;

public class StructureSlotUI : SlotUI
{
    [SerializeField] private StructureEntryUI _entryPrefab;
    [SerializeField] private GameObject _buildIcon;

    private PlayerBuildingManager _buildingManager;
    private List<StructureEntryUI> _entryUIList = new();
    private bool _cashedCanBuild = true;

    public void Init(PlayerBuildingManager buildingManager)
    {
        _buildingManager = buildingManager;
    }

    private void Start()
    {
        for (int i = 0; i < _buildingManager.Entries.Count; i++)
        {
            StructureEntryUI entryUI = Instantiate(_entryPrefab, _slotContents.transform);
            _entryUIList.Add(entryUI);
            string keyName = (i + 1).ToString();
            entryUI.Init(_buildingManager.Entries[i], keyName);
        }

        _buildingManager.OnSelectedStructureChanged += UpdateSelectedEntry;

        _cashedCanBuild = _buildingManager.CanBuildSelectedStructure;
    }

    private void OnDestroy()
    {
        _buildingManager.OnSelectedStructureChanged -= UpdateSelectedEntry;
    }

    private void LateUpdate()
    {
        if (_buildingManager.CanBuildSelectedStructure != _cashedCanBuild)
        {
            _cashedCanBuild = _buildingManager.CanBuildSelectedStructure;
            _buildIcon.SetActive(_cashedCanBuild);
        }
    }

    protected override void UpdateSelectedEntry(int index)
    {
        for (int i = 0; i < _entryUIList.Count; i++)
        {
            bool isSelected = i == index;
            _entryUIList[i].SetSelected(isSelected);
        }
    }

    public override void Focused()
    {
        _buildIcon.SetActive(true);
    }

    public override void Unfocused()
    {
        _buildIcon.SetActive(false);
        for (int i = 0; i < _entryUIList.Count; i++)
        {
            _entryUIList[i].SetSelected(false);
        }
    }
}
