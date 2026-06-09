using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponBase[] _weapons;
    // -1 means no weapon is currently equipped
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

        // Start with sword selected, but keep it hidden until an attack happens.
        if (_weapons != null && _weapons.Length > 0)
        {
            SelectWeapon(0);
        }
    }

    public void EnterBuildingMode()
    {
        UnequipCurrentWeapon();
    }

    public void ExitBuildingMode()
    {
        // 建築モードから抜けたときは、次の攻撃入力まで非表示のままにする
        UnequipCurrentWeapon();
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

    // Equip a weapon without triggering its attack. index must be valid.
    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= _weapons.Length) return;

        SelectWeapon(index);
        _weapons[_currentWeaponIndex].Equip();
    }

    public bool TryUseSelectedWeapon()
    {
        if (_weapons == null || _weapons.Length == 0) return false;
        if (_currentWeaponIndex < 0 || _currentWeaponIndex >= _weapons.Length) return false;

        if (_weapons[_currentWeaponIndex].CurrentState != WeaponBase.WeaponState.Idle)
            return false;

        _weapons[_currentWeaponIndex].Equip();
        return _weapons[_currentWeaponIndex].TryUseWeapon();
    }

    // Use the currently equipped weapon (without changing the equipped index)
    public bool TryUseEquippedWeapon()
    {
        if (_weapons == null || _weapons.Length == 0) return false;
        if (_currentWeaponIndex < 0 || _currentWeaponIndex >= _weapons.Length) return false;
        return _weapons[_currentWeaponIndex].TryUseWeapon();
    }

    private bool TryUseWeapon(int index)
    {
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

    public void HideCurrentWeapon()
    {
        UnequipCurrentWeapon();
    }

    private void HandleWeaponAttackCompleted(WeaponBase weapon)
    {
        if (_currentWeaponIndex < 0 || _currentWeaponIndex >= _weapons.Length) return;
        if (_weapons[_currentWeaponIndex] != weapon) return;

        weapon.Unequip();
    }
}
