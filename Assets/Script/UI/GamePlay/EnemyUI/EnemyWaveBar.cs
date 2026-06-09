using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaveProgressBar : MonoBehaviour
{
    [SerializeField] private Image _waveBarFill;

    private void Start()
    {
        GameManager.Instance.OnEnemyWaveProgressChanged += UpdateWaveBar;
        UpdateWaveBar(GameManager.Instance.EnemyWaveProgress);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnEnemyWaveProgressChanged -= UpdateWaveBar;
    }

    private void UpdateWaveBar(float ratio)
    {
        _waveBarFill.fillAmount = 1 - ratio;
    }
}
