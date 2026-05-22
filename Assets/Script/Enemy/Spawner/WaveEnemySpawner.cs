using System;
using System.Linq;
using UnityEngine;

public class WaveEnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private int _firstIndex = 0;
    [SerializeField] private WaveData _waveData;
    [SerializeField] private CurrencyWallet _playerCurrencyWallet;
    [SerializeField] private int _killExperience = 1; //TODO:個別で経験値を設定する場合は経験値処理を分ける

    private int _maxEnemyCount = 0;
    private int _killedEnemyCount = 0;
    private float _timer = 0;
    private int _enemyIndex = 0;

    public float ProgressRatio => (float)_killedEnemyCount / (_maxEnemyCount != 0 ? _maxEnemyCount : 1);
    public event Action<float> OnProgressChanged;

    private void Start()
    {
        _enemyIndex = _firstIndex;
        _maxEnemyCount = _waveData.EnemyList.Length != 0 ? _waveData.EnemyList.Length : 1;
    }

    private void Update()
    {
        if (_enemyIndex >= _waveData.EnemyList.Length) return;

        _timer += Time.deltaTime;

        WaveData.EnemySpawnInfo spawnInfo = _waveData.EnemyList[_enemyIndex];
        if (spawnInfo.SpawnDelaySecond <= _timer)
        {
            SpawnEnemy(spawnInfo.EnemyPrefab);
            _enemyIndex++;

            _timer = 0;
        }
    }

    protected void SpawnEnemy(GameObject enemyPre)
    {
        GameObject enemy = Instantiate(enemyPre, transform.position, transform.rotation, transform);
        enemy.GetComponent<CharacterAIController>().Init(_target);
        enemy.GetComponent<Health>().OnDied += OnEnemyKilled;

    }

    private void OnEnemyKilled()
    {
        _killedEnemyCount++;
        _playerCurrencyWallet.AddCurrency(CurrencyData.CurrencyType.Experience, _killExperience);
        OnProgressChanged?.Invoke(ProgressRatio);
    }
    private bool CheckStageClear()
    {
        if (transform.childCount <= 0)
        {
            return true;
        }

        return false;
    }
}