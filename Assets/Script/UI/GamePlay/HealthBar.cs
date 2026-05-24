using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform _HealthBarFill;
    [SerializeField] private Health _health;

    private void Awake()
    {
        _health.OnHealthChanged += UpdateHealthBar;
    }
    private void OnDestroy()
    {
        _health.OnHealthChanged -= UpdateHealthBar;
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    private void UpdateHealthBar(float ratio)
    {
        if (_HealthBarFill != null)
        {
            _HealthBarFill.localScale = new Vector3(ratio, 1f, 1f);
        }
    }
}
