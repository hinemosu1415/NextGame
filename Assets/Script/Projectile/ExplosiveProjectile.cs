using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] protected float _explosionDamage;
    [SerializeField] protected Explosion _explosionPrefab;

    public void Init(float damage, float explosionDamage)
    {
        base.Init(damage);
        _explosionDamage = explosionDamage;
    }

    protected override void OnDestroy()
    {
        // 爆発を生成
        if (_explosionPrefab != null)
        {
            Explosion explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.Init(_explosionDamage);
        }

        base.OnDestroy();
    }
}
