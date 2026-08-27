using System;
using System.Linq;
using UnityEngine;

public class WaveEnemySpawner : MonoBehaviour, IWaveProgressProvider
{
    [SerializeField] private Transform _target;
    [SerializeField] private int _firstIndex = 0;//最初からスポーンする敵のインデックス(デバッグ用)
    [SerializeField] private WaveData _waveData;
    [SerializeField] private CurrencyWallet _playerCurrencyWallet;
    [SerializeField] private int _killExperience = 1; //TODO:個別で経験値を設定する場合は経験値処理を分ける
    [SerializeField] private bool _registerToGameManager = true;
    [SerializeField] private bool _autoSpawnOnStart = true;

    private int _maxEnemyCount = 0;
    private int _killedEnemyCount = 0;
    private float _timer = 0;
    private int _enemyIndex = 0;
    private bool _autoSpawnEnabled;
    private bool _isInitialized;

    public float ProgressRatio => (float)_killedEnemyCount / (_maxEnemyCount != 0 ? _maxEnemyCount : 1);
    public event Action<float> OnProgressChanged;
    public event Action<Health> OnEnemySpawned;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();

        if (_registerToGameManager && GameManager.Instance != null)
            GameManager.Instance.EnemyWaveProgressProviders.Add(this);
    }

    private void OnDestroy()
    {
        if (_registerToGameManager && GameManager.Instance != null)
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
            SpawnEnemy(spawnInfo.EnemyPrefab);
            _enemyIndex++;

            _timer = 0;
        }
    }

    public void SetAutoSpawnEnabled(bool enabled)
    {
        Initialize();
        _autoSpawnEnabled = enabled;
    }

    public int SpawnImmediateFromCurrentWave(int spawnCount)
    {
        Initialize();
        if (_waveData == null || _waveData.EnemyList == null || spawnCount <= 0)
            return 0;

        int spawned = 0;
        for (int i = 0; i < spawnCount; i++)
        {
            if (_enemyIndex >= _waveData.EnemyList.Length) break;

            WaveData.EnemySpawnInfo spawnInfo = _waveData.EnemyList[_enemyIndex];
            SpawnEnemy(spawnInfo.EnemyPrefab);
            _enemyIndex++;
            spawned++;
        }

        return spawned;
    }

    public Health SpawnImmediate(GameObject enemyPrefab)
    {
        if (enemyPrefab == null) return null;
        return SpawnEnemy(enemyPrefab);
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

    private void Initialize()
    {
        if (_isInitialized) return;

        _isInitialized = true;
        _autoSpawnEnabled = _autoSpawnOnStart;

        if (_waveData == null || _waveData.EnemyList == null)
        {
            _enemyIndex = 0;
            _maxEnemyCount = 0;
            return;
        }

        _enemyIndex = Mathf.Clamp(_firstIndex, 0, _waveData.EnemyList.Length);
        _maxEnemyCount = _waveData.EnemyList.Length - _enemyIndex;
    }
}
