using UnityEngine;
using Utility;

public abstract class Structure : MonoBehaviour
{
    [Header("時間設定")]
    [SerializeField] protected float _executeInterval = 2.0f;

    private Health _health;
    protected GameObject _player;
    protected CountdownTimer _cooldownTimer;

    protected void Awake()
    {
        _health = GetComponent<Health>();
    }
    protected void Start()
    {
        if (_health != null)
        {
            _health.OnDied += Die;
        }
        _cooldownTimer = new CountdownTimer(_executeInterval);
    }

    public virtual void Init(GameObject player)
    {
        _player = player;
    }

    protected virtual void Update()
    {
        _cooldownTimer.Tick();
        if (_cooldownTimer.IsFinished())
        {
            Execute();
            _cooldownTimer.Start();
        }
    }

    protected abstract void Execute();

    protected void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDied -= Die;
        }
    }
    protected void Die()
    {
        Destroy(gameObject);
    }
}