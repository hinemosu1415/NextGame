using System.Collections.Generic;
using UnityEngine;

public class BuildingModeUI : MonoBehaviour
{
    [SerializeField] private StructureEntryUI _entryPrefab;
    [SerializeField] private PlayerBuildingManager _buildingManager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _structureSlot;
    [SerializeField] private GameObject _buildIcon;

    private List<StructureEntryUI> _entryUIList = new();
    private bool _cashedCanBuild = true;

    private void Start()
    {
        for (int i = 0; i < _buildingManager.Entries.Count; i++)
        {
            StructureEntryUI entryUI = Instantiate(_entryPrefab, _structureSlot.transform);
            _entryUIList.Add(entryUI);
            string keyName = (i + 1).ToString();
            entryUI.Init(_buildingManager.Entries[i], keyName);
        }

        _buildingManager.OnSelectedStructureChanged += UpdateSelectedStructure;
        _playerController.OnModeChanged += SetActiveByMode;

        UpdateSelectedStructure(0);
        SetActiveByMode(_playerController.CurrentMode);
        _cashedCanBuild = _buildingManager.CanBuildSelectedStructure;
    }

    private void OnDestroy()
    {
        _buildingManager.OnSelectedStructureChanged -= UpdateSelectedStructure;
        _playerController.OnModeChanged -= SetActiveByMode;
    }

    private void SetActiveByMode(PlayerController.Mode mode)
    {
        if (mode == PlayerController.Mode.Building)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (_buildingManager.CanBuildSelectedStructure != _cashedCanBuild)
        {
            _cashedCanBuild = _buildingManager.CanBuildSelectedStructure;
            _buildIcon.SetActive(_cashedCanBuild);
        }
    }

    private void UpdateSelectedStructure(int index)
    {
        for (int i = 0; i < _entryUIList.Count; i++)
        {
            bool isSelected = i == index;
            _entryUIList[i].SetSelected(isSelected);
        }
    }
}
