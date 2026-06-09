using System.Collections;
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected WeaponData _weaponData;

    private SpriteRenderer _renderer;

    public enum WeaponState
    {
        Idle,
        Attacking,
        CoolingDown
    }

    public WeaponState CurrentState { get; private set; } = WeaponState.Idle;

    public event Action<WeaponBase> OnAttackCompleted;

    public string WeaponName => _weaponData.WeaponName;

    protected virtual void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public bool TryUseWeapon()
    {
        if (CurrentState != WeaponState.Idle) return false;

        StartCoroutine(AttackAndCooldown());

        return true;
    }

    private IEnumerator AttackAndCooldown()
    {
        CurrentState = WeaponState.Attacking;
        yield return AttackCoroutine();

        OnAttackCompleted?.Invoke(this);

        CurrentState = WeaponState.CoolingDown;
        yield return new WaitForSeconds(_weaponData.CoolTime);

        CurrentState = WeaponState.Idle;
    }

    protected abstract IEnumerator AttackCoroutine();

    public virtual void Equip()
    {
        _renderer.enabled = true;
    }

    public virtual void Unequip()
    {
        _renderer.enabled = false;
    }
}