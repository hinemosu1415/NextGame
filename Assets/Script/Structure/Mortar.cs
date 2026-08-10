using UnityEngine;

public class Mortar : Structure
{
    [SerializeField] private ExplosiveProjectile _projectilePrefab;
    [SerializeField] private float _damageAmount;
    [SerializeField] private float _explosionDamageAmount;
    [SerializeField] private float _spawnForceYMin;
    [SerializeField] private float _spawnForceYMax;
    [SerializeField] private float _targetRange;

    protected override void Execute()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, _targetRange);
        if (enemiesInRange.Length == 0) return;
        GameObject target = null;
        foreach (var enemy in enemiesInRange)
        {
            if (enemy.CompareTag("Enemy"))
            {
                target = enemy.gameObject;
                break;
            }
        }
        if (target == null) return;

        ExplosiveProjectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        projectile.Init(_damageAmount, _explosionDamageAmount);

        float spawnForceY = Random.Range(_spawnForceYMin, _spawnForceYMax);
        const float SPAWN_LEAD_DISTANCE_FACTOR = 0.8f; //目標に対しての偏差打ちを行うための係数
        float spawnForceX = (target.transform.position.x - transform.position.x) * SPAWN_LEAD_DISTANCE_FACTOR;
        projectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(spawnForceX, spawnForceY), ForceMode2D.Impulse);
    }
}