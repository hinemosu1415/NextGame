using UnityEngine;
using System.Collections;

public class CharacterDeathAnimator : MonoBehaviour
{
    [SerializeField] private float _knockbackForce = 5f;
    [SerializeField] private float _rotationSpeed = 100f;

    private readonly Vector2 KNOCKBACK_LEFT = Quaternion.Euler(0, 0, 135) * Vector2.right;
    private readonly Vector2 KNOCKBACK_RIGHT = Quaternion.Euler(0, 0, 45) * Vector2.right;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void PlayDeathAnimation(float destroyDelay)
    {
        _rigidbody.constraints = RigidbodyConstraints2D.None;

        Vector2 knockbackDirection = GetKnockbackDirection();

        _rigidbody.linearVelocity = knockbackDirection * _knockbackForce;
        transform.rotation = Quaternion.Euler(0, knockbackDirection.x < 0 ? 0 : 180, 0);
        StartCoroutine(AnimateDeathRotation(_rotationSpeed));

        Destroy(gameObject, destroyDelay);
    }

    private Vector2 GetKnockbackDirection()
    {
        const float SEARCH_RADIUS = 1f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, SEARCH_RADIUS);
        Vector3 selfPos = transform.position;
        Collider2D closestTrigger = null;
        float closestDistanceSq = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.isTrigger)
            {
                float distanceSq = ((Vector3)collider.bounds.center - selfPos).sqrMagnitude;
                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closestTrigger = collider;
                }
            }
        }

        if (closestTrigger != null)
        {
            return closestTrigger.bounds.center.x > transform.position.x ? KNOCKBACK_LEFT : KNOCKBACK_RIGHT;
        }

        return Vector2.up;
    }

    private IEnumerator AnimateDeathRotation(float speed)
    {
        while (gameObject != null)
        {
            transform.rotation *= Quaternion.Euler(0, 0, speed * Time.deltaTime);
            yield return null;
        }
    }
}




