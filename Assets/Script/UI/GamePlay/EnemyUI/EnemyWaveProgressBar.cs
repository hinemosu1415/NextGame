using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaveBar : MonoBehaviour
{
    [SerializeField] WaveEnemySpawner _enemySpawner;
    [SerializeField] private Image _waveBarFill;

    private void Start()
    {
        _enemySpawner.OnProgressChanged += UpdateWaveBar;
        UpdateWaveBar(_enemySpawner.ProgressRatio);
    }

    private void OnDestroy()
    {
        _enemySpawner.OnProgressChanged -= UpdateWaveBar;
    }

    private void UpdateWaveBar(float ratio)
    {
        _waveBarFill.fillAmount = 1 - ratio;
    }
}
