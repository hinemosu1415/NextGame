using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponBase[] _weapons;
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
        foreach (var weapon in _weapons)
        {
            weapon.Unequip();
            weapon.OnAttackCompleted += HandleWeaponAttackCompleted;
        }

        // デフォルトは剣、攻撃が発生するまでは剣を隠しておく。
        if (_weapons != null && _weapons.Length > 0)
        {
            SelectWeapon(0);
        }
    }

    private void OnDestroy()
    {
        if (_weapons == null) return;

        foreach (var weapon in _weapons)
        {
            if (weapon != null)
                weapon.OnAttackCompleted -= HandleWeaponAttackCompleted;
        }
    }

    public bool TryUsePrimaryWeapon()
    {
        return TryUseWeapon(0);
    }
    public bool TryUseSecondaryWeapon()
    {
        return TryUseWeapon(1);
    }

    public void SelectWeapon(int index)
    {
        if (index < 0 || index >= _weapons.Length) return;

        if (_currentWeaponIndex != index && _currentWeaponIndex >= 0 && _currentWeaponIndex < _weapons.Length)
            _weapons[_currentWeaponIndex].Unequip();

        _currentWeaponIndex = index;
    }

    // 選択した武器を装備する（攻撃は別途TryUseSelectedWeaponで行う）
    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= _weapons.Length) return;

        SelectWeapon(index);
        _weapons[_currentWeaponIndex].Equip();
    }

    public bool TryUseSelectedWeapon()
    {
        return TryUseWeapon(_currentWeaponIndex);
    }

    private bool TryUseWeapon(int index)
    {
        if (_weapons == null || _weapons.Length == 0) return false;
        if (index < 0 || index >= _weapons.Length) return false;

        if (CurrentWeaponState != WeaponBase.WeaponState.Idle) return false;

        if (_currentWeaponIndex != index && _currentWeaponIndex >= 0 && _currentWeaponIndex < _weapons.Length)
            _weapons[_currentWeaponIndex].Unequip();

        _currentWeaponIndex = index;

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
        if (_currentWeaponIndex < 0 || _currentWeaponIndex >= _weapons.Length) return;
        if (_weapons[_currentWeaponIndex] != weapon) return;

        weapon.Unequip();
    }
}
