using System.Collections;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private float _lifetime = 10f;
    [SerializeField] private float _invincibilityDuration = 0.1f;

    [SerializeField] private AudioClip _pickupSound;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        StartCoroutine(StartInvincibility());
        Destroy(gameObject, _lifetime);
    }

    private IEnumerator StartInvincibility()
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(_invincibilityDuration);
        _collider.enabled = true;
    }

    public void Consume(GameObject collector)
    {
        if (_pickupSound != null)
        {
            AudioManager.Instance.PlaySoundEffect(_pickupSound);
        }

        Execute(collector);
        Dispose();
    }

    protected virtual void Dispose()
    {
        Destroy(gameObject);
    }

    protected abstract void Execute(GameObject target);
}