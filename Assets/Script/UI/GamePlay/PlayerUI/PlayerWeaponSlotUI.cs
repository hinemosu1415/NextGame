using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSlotUI : SlotUI
{
    [SerializeField] private PlayerWeaponEntryUI _entryPrefab;
    [SerializeField] private GameObject _attackIcon;

    private PlayerWeaponManager _weaponManager;
    private List<PlayerWeaponEntryUI> _entryUIList = new();

    public void Init(PlayerWeaponManager playerWeaponManager)
    {
        _weaponManager = playerWeaponManager;
    }


    private void Start()
    {
        for (int i = 0; i < _weaponManager.PlayerWeaponDataArray.Length; i++)
        {
            PlayerWeaponEntryUI entryUI = Instantiate(_entryPrefab, _slotContents.transform);
            _entryUIList.Add(entryUI);
            entryUI.Init(_weaponManager.PlayerWeaponDataArray[i], i == 0 ? "E" : "R"); //TODO: キーバインドからキーの割り当てを行うように変更する
        }

        _weaponManager.OnSelectedStructureChanged += UpdateSelectedEntry;

        UpdateSelectedEntry(0);
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
        _attackIcon.SetActive(true);
    }
    public override void Unfocused()
    {
        _attackIcon.SetActive(false);
        for (int i = 0; i < _entryUIList.Count; i++)
        {
            _entryUIList[i].SetSelected(false);
        }
    }
}
