using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        _health.OnDied += () => HandleRocketDestroyed();
    }

    private void OnDestroy()
    {
        _health.OnDied -= () => HandleRocketDestroyed();
    }

    private void HandleRocketDestroyed()
    {
        GameManager.Instance.OnRocketDestroyed();
    }
}
