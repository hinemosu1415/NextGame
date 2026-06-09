using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class HealthBarCanvas : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private float _speed = 5f; // 大きいほど速い

    private float _targetRatio;
    private bool _isOverrideAnimating;
    private Coroutine _overrideRoutine;

    private void Start()
    {
        _health.OnHealthChanged += OnHealthChanged;
        _targetRatio = _health.HealthRatio;
        _healthBarFill.fillAmount = _targetRatio;
    }

    private void OnDestroy()
    {
        _health.OnHealthChanged -= OnHealthChanged;
    }

    private void Update()
    {
        if (_isOverrideAnimating)
        {
            return;
        }

        _healthBarFill.fillAmount = Mathf.Lerp(
            _healthBarFill.fillAmount,
            _targetRatio,
            Time.deltaTime * _speed
        );
    }

    private void OnHealthChanged(float ratio)
    {
        _targetRatio = ratio;
    }

    public void PlayLinearFill(float targetRatio, float duration)
    {
        if (_overrideRoutine != null)
        {
            StopCoroutine(_overrideRoutine);
        }

        _overrideRoutine = StartCoroutine(PlayLinearFillRoutine(targetRatio, duration));
    }

    private IEnumerator PlayLinearFillRoutine(float targetRatio, float duration)
    {
        _isOverrideAnimating = true;

        float startRatio = _healthBarFill.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            _healthBarFill.fillAmount = Mathf.Lerp(startRatio, targetRatio, t);
            yield return null;
        }

        _healthBarFill.fillAmount = targetRatio;
        _targetRatio = targetRatio;
        _isOverrideAnimating = false;
        _overrideRoutine = null;
    }
}