using UnityEngine;
using Utility;

public class MissileBullet : ExplosiveProjectile
{
    [Header("MissileBullet")]
    [SerializeField] protected float _rotateSpeed = 200f;
    [SerializeField] protected float _homingDelay = 0.5f;

    private Transform target;
    private CountdownTimer timer;

    private enum State { Launch, Homing }
    private State state = State.Launch;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    protected override void Start()
    {
        base.Start();
        timer = new CountdownTimer(_homingDelay);
    }

    protected override void Move()
    {
        timer.Tick();

        // 一定時間後に追尾開始
        if (timer.IsFinished())
        {
            state = State.Homing;
        }

        // 追尾処理
        if (state == State.Homing && target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;

            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            float angle = Mathf.MoveTowardsAngle(
                transform.eulerAngles.z,
                targetAngle,
                _rotateSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // 常に前進（2Dは right が前）
        transform.position += transform.right * _speed * Time.deltaTime;
    }
}
