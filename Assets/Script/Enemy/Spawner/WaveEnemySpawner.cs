using System;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveEnemySpawner : MonoBehaviour, IWaveProgressProvider
{
    [SerializeField] private Transform _target;
    [SerializeField] private int _firstIndex = 0;//最初からスポーンする敵のインデックス(デバッグ用)
    [SerializeField] private WaveData _waveData;
    [SerializeField] private CurrencyWallet _playerCurrencyWallet;
    [SerializeField] private int _killExperience = 1; //TODO:個別で経験値を設定する場合は経験値処理を分ける
    [FormerlySerializedAs("_registerToGameManager")]
    [SerializeField] private bool _contributesToGameClearProgress = true;
    [SerializeField] private bool _autoSpawnOnStart = true;

    private int _maxEnemyCount = 0;
    private int _killedEnemyCount = 0;
    private float _timer = 0;
    private int _enemyIndex = 0;
    private bool _autoSpawnEnabled;

    public float ProgressRatio => (float)_killedEnemyCount / (_maxEnemyCount != 0 ? _maxEnemyCount : 1);
    public event Action<float> OnProgressChanged;
    public event Action<Health> OnEnemySpawned;

    private void Awake()
    {
        _autoSpawnEnabled = _autoSpawnOnStart;
        _enemyIndex = Mathf.Clamp(_firstIndex, 0, _waveData.EnemyList.Length);
        _maxEnemyCount = _waveData.EnemyList.Length - _enemyIndex;
    }

    private void Start()
    {
        if (_contributesToGameClearProgress)
            GameManager.Instance.EnemyWaveProgressProviders.Add(this);
    }

    private void OnDestroy()
    {
        if (_contributesToGameClearProgress)
            GameManager.Instance.EnemyWaveProgressProviders.Remove(this);
    }

    private void Update()
    {
        if (!_autoSpawnEnabled) return;
        if (_enemyIndex >= _waveData.EnemyList.Length) return;

        _timer += Time.deltaTime;

        WaveData.EnemySpawnInfo spawnInfo = _waveData.EnemyList[_enemyIndex];
        if (spawnInfo.SpawnDelaySecond <= _timer)
        {
            SpawnNextEnemy();
            _timer = 0;
        }
    }

    public void SetAutoSpawn(bool enabled)
    {
        _autoSpawnEnabled = enabled;
    }

    public Health SpawnNextEnemy()
    {
        if (_enemyIndex >= _waveData.EnemyList.Length) return null;

        WaveData.EnemySpawnInfo spawnInfo = _waveData.EnemyList[_enemyIndex];
        _enemyIndex++;
        return SpawnEnemy(spawnInfo.EnemyPrefab);
    }

    protected Health SpawnEnemy(GameObject enemyPre)
    {
        GameObject enemy = Instantiate(enemyPre, transform.position, transform.rotation, transform);
        enemy.GetComponent<CharacterAIController>().Init(_target);
        Health enemyHealth = enemy.GetComponent<Health>();
        enemyHealth.OnDied += OnEnemyKilled;
        OnEnemySpawned?.Invoke(enemyHealth);
        return enemyHealth;
    }

    private void OnEnemyKilled()
    {
        _killedEnemyCount++;
        if (_playerCurrencyWallet != null)
            _playerCurrencyWallet.AddCurrency(CurrencyData.CurrencyType.Experience, _killExperience);
        OnProgressChanged?.Invoke(ProgressRatio);
    }

}
