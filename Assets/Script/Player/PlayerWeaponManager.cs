using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] private PlayerWeaponData[] _playerWeaponDataArray;
    [SerializeField] private Transform _weaponParent;
    private WeaponBase[] _weapons;

    // -1は、現在武器が装備されていないことを意味します。
    private int _currentWeaponIndex = -1;

    public WeaponBase.WeaponState CurrentWeaponState =>
        (_currentWeaponIndex >= 0 && _weapons != null && _currentWeaponIndex < _weapons.Length)
            ? _weapons[_currentWeaponIndex].CurrentState
            : WeaponBase.WeaponState.Idle;

    public string GetCurrentWeaponName =>
        (_currentWeaponIndex >= 0 && _weapons != null && _currentWeaponIndex < _weapons.Length)
            ? _weapons[_currentWeaponIndex].WeaponName
            : string.Empty;

    private void Start()
    {
        InitWeapons();
        // デフォルトは0、攻撃が発生するまでは武器を隠しておく。
        if (_weapons != null && _weapons.Length > 0)
        {
            SelectWeapon(0);
        }
    }

    private void OnDestroy()
    {
        foreach (var weapon in _weapons)
        {
            weapon.OnAttackCompleted -= HandleWeaponAttackCompleted;
        }
    }

    private void InitWeapons()
    {
        _weapons = new WeaponBase[_playerWeaponDataArray.Length];
        for (int i = 0; i < _playerWeaponDataArray.Length; i++)
        {
            if (_playerWeaponDataArray[i] != null)
            {
                WeaponBase weaponBase = Instantiate(_playerWeaponDataArray[i].weaponBase, _weaponParent);

                weaponBase.transform.localPosition = _playerWeaponDataArray[i].EquippedOffset;
                _weapons[i] = weaponBase;
                _weapons[i].OnAttackCompleted += HandleWeaponAttackCompleted;
                _weapons[i].Unequip();
            }
        }
    }

    public void SelectWeapon(int index)
    {
        if (index < 0 || index >= _weapons.Length) return;

        UnequipCurrentWeapon();

        _currentWeaponIndex = index;
    }

    public bool TryUseSelectedWeapon()
    {
        if (CurrentWeaponState != WeaponBase.WeaponState.Idle) return false;

        _weapons[_currentWeaponIndex].Equip();
        return _weapons[_currentWeaponIndex].TryUseWeapon();
    }

    public void UnequipCurrentWeapon()
    {
        if (_currentWeaponIndex >= 0 && _currentWeaponIndex < _weapons.Length)
            _weapons[_currentWeaponIndex].Unequip();
    }

    private void HandleWeaponAttackCompleted(WeaponBase weapon)
    {
        weapon.Unequip();
    }
}
