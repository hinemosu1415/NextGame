using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] protected float _speed = 12f;
    [SerializeField] protected float _lifeTime = 5f;

    [Header("Hit")]
    [SerializeField] protected float _damageAmount;
    [SerializeField] protected bool _destroyOnHit = true;
    [SerializeField] protected float _groundHitStop;

    [Header("Collision")]
    [SerializeField] protected LayerMask _groundLayer;

    protected Hitbox[] _hitbox;
    protected bool _isMoving = true;

    protected virtual void Awake()
    {
        _hitbox = GetComponentsInChildren<Hitbox>(true);
    }

    public virtual void Init(float damage)
    {
        _damageAmount = damage;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, _lifeTime);
        foreach (var hitbox in _hitbox)
        {
            hitbox.OnFirstHit += OnHit;
        }
    }

    protected virtual void OnDestroy()
    {
        foreach (var hitbox in _hitbox)
        {
            hitbox.OnFirstHit -= OnHit;
        }
    }

    protected virtual void Update()
    {
        if (_isMoving)
        {
            Move();
        }
    }

    protected virtual void Move()
    {
        // FirePoint の右方向へ直進
        transform.Translate(Vector2.right * _speed * Time.deltaTime);
    }

    protected virtual void OnHit(Health targetHealth)
    {
        // ヒットした相手にダメージを与える
        targetHealth.TakeDamage(_damageAmount);

        // ヒット後の処理
        if (_destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 指定された Layer に接触したら削除
        if ((_groundLayer & (1 << collision.gameObject.layer)) != 0)
        {
            _isMoving = false; // 移動停止
            Destroy(gameObject, _groundHitStop);
        }
    }
}
