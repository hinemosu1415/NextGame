using System.Collections;
using UnityEngine;

public class Bomb : WeaponBase
{
    [SerializeField] private Explosion _explosionPre;
    [SerializeField] private string _attackAnimationClipName = "Attack";
    private Animator _animator;
    private float _attackDuration;
    readonly int _hashAttack = Animator.StringToHash("Attack");

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        int animationHash = Animator.StringToHash(_attackAnimationClipName);
        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == animationHash) _attackDuration = clip.length;
        }
    }

    protected override IEnumerator AttackCoroutine()
    {
        Explosion explosion = Instantiate(_explosionPre, transform.position, transform.rotation);
        explosion.Init(_weaponData.DamageAmount);

        _animator.SetTrigger(_hashAttack);
        yield return new WaitForSeconds(_attackDuration);
    }
}
