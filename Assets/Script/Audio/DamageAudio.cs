using UnityEngine;

public class DamageAudio : MonoBehaviour
{
    [SerializeField] private AudioClip _damageSound;
    [SerializeField] private AudioClip _DieSound;

    private Health _health;
    private float _prevRatio = 1f;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnHealthChanged += PlayDamageSound;
        _health.OnDied += PlayDieSound;
    }

    private void OnDestroy()
    {
        _health.OnHealthChanged -= PlayDamageSound;
        _health.OnDied -= PlayDieSound;
    }

    private void PlayDamageSound(float ratio)
    {
        if (ratio < _prevRatio)
        {
            AudioManager.Instance.PlaySoundEffect(_damageSound);
        }
        _prevRatio = ratio;
    }

    private void PlayDieSound()
    {
        AudioManager.Instance.PlaySoundEffect(_DieSound);
    }
}