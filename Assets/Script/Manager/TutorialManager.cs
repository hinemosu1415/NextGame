using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TutorialStep
{
    Move,
    Jump,
    HealthExplanation,
    WaveExplanation,
    Attack,
    ChangeMode,
    PlaceStructure,
    SummonAlly,
    Boss,
    Complete
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private PlayerBuildingManager _buildingManager;
    [SerializeField] private PlayerAllyManager _allyManager;
    [SerializeField] private Health _dummyEnemy;
    [SerializeField] private TutorialUI _tutorialUI;
    [SerializeField] private float _requiredMoveDistance = 3f;
    [Header("Tutorial enemy wave")]
    [SerializeField] private WaveEnemySpawner _waveEnemySpawner;
    [SerializeField, Min(1)] private int _enemySpawnCount = 1;
    [SerializeField] private GameObject _bossPrefab;

    public TutorialStep CurrentStep { get; private set; } = TutorialStep.Move;

    private float _startX;
    private float _dummyEnemyHealthRatio;
    private bool _isChangingStep;
    private bool _hasSpawnedTutorialEnemies;
    private Health _bossHealth;
    private bool _hasSpawnedBoss;

    private void Awake()
    {
        if (_player == null) _player = FindFirstObjectByType<PlayerController>();
        if (_buildingManager == null && _player != null) _buildingManager = _player.GetComponent<PlayerBuildingManager>();
        if (_allyManager == null && _player != null) _allyManager = _player.GetComponent<PlayerAllyManager>();
        if (_waveEnemySpawner == null) _waveEnemySpawner = FindFirstObjectByType<WaveEnemySpawner>();
    }

    private void Start()
    {
        if (_player == null || _tutorialUI == null)
        {
            Debug.LogError("TutorialManager requires a PlayerController and TutorialUI reference.");
            enabled = false;
            return;
        }

        _startX = _player.transform.position.x;
        if (_waveEnemySpawner != null)
            _waveEnemySpawner.SetAutoSpawnEnabled(false);
        _player.OnJumped += CompleteJump;
        _player.OnPrimaryAttacked += CompleteAttackInput;
        _player.OnModeChanged += CompleteModeChange;
        if (_buildingManager != null) _buildingManager.OnStructurePlaced += CompleteStructurePlacement;
        if (_allyManager != null) _allyManager.OnAllySpawned += CompleteAllySummon;
        SubscribeToAttackTarget(_dummyEnemy);
        if (_waveEnemySpawner != null)
            _waveEnemySpawner.OnEnemySpawned += HandleEnemySpawned;

        ShowCurrentStep();
    }

    private void Update()
    {
        if (CurrentStep == TutorialStep.HealthExplanation ||
            CurrentStep == TutorialStep.WaveExplanation)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null &&
                (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame))
            {
                TutorialStep nextStep = CurrentStep == TutorialStep.HealthExplanation
                    ? TutorialStep.WaveExplanation
                    : TutorialStep.Attack;
                AdvanceTo(nextStep);
            }
            return;
        }

        if (CurrentStep != TutorialStep.Move || _player == null) return;

        if (Mathf.Abs(_player.transform.position.x - _startX) >= _requiredMoveDistance)
            AdvanceTo(TutorialStep.Jump);
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnJumped -= CompleteJump;
            _player.OnPrimaryAttacked -= CompleteAttackInput;
            _player.OnModeChanged -= CompleteModeChange;
        }
        if (_buildingManager != null) _buildingManager.OnStructurePlaced -= CompleteStructurePlacement;
        if (_allyManager != null) _allyManager.OnAllySpawned -= CompleteAllySummon;
        if (_dummyEnemy != null) _dummyEnemy.OnHealthChanged -= HandleDummyEnemyHealthChanged;
        if (_waveEnemySpawner != null) _waveEnemySpawner.OnEnemySpawned -= HandleEnemySpawned;
        if (_bossHealth != null) _bossHealth.OnDied -= CompleteBossBattle;
    }

    private void CompleteJump()
    {
        if (CurrentStep == TutorialStep.Jump) AdvanceTo(TutorialStep.HealthExplanation);
    }

    private void CompleteAttackInput()
    {
        // The dummy's health change confirms that the attack reached its target.
    }

    private void HandleDummyEnemyHealthChanged(float healthRatio)
    {
        if (CurrentStep == TutorialStep.Attack && healthRatio < _dummyEnemyHealthRatio)
            AdvanceTo(TutorialStep.ChangeMode);
        _dummyEnemyHealthRatio = healthRatio;
    }

    private void SubscribeToAttackTarget(Health enemyHealth)
    {
        if (enemyHealth == null) return;
        if (_dummyEnemy != null)
            _dummyEnemy.OnHealthChanged -= HandleDummyEnemyHealthChanged;

        _dummyEnemy = enemyHealth;
        _dummyEnemyHealthRatio = _dummyEnemy.HealthRatio;
        _dummyEnemy.OnHealthChanged += HandleDummyEnemyHealthChanged;
    }

    private void HandleEnemySpawned(Health enemyHealth)
    {
        if (CurrentStep == TutorialStep.Attack)
            SubscribeToAttackTarget(enemyHealth);
    }

    private void CompleteModeChange(PlayerController.Mode mode)
    {
        if (CurrentStep == TutorialStep.ChangeMode && mode == PlayerController.Mode.Building)
            AdvanceTo(TutorialStep.PlaceStructure);
    }

    private void CompleteStructurePlacement()
    {
        if (CurrentStep == TutorialStep.PlaceStructure) AdvanceTo(TutorialStep.SummonAlly);
    }

    private void CompleteAllySummon()
    {
        if (CurrentStep == TutorialStep.SummonAlly) AdvanceTo(TutorialStep.Boss);
    }

    private void CompleteBossBattle()
    {
        if (_bossHealth != null)
            _bossHealth.OnDied -= CompleteBossBattle;

        if (CurrentStep == TutorialStep.Boss)
            AdvanceTo(TutorialStep.Complete);
    }

    private void AdvanceTo(TutorialStep nextStep)
    {
        if (_isChangingStep || (int)nextStep <= (int)CurrentStep) return;

        _isChangingStep = true;
        CurrentStep = nextStep;
        ShowCurrentStep();
        _isChangingStep = false;
    }

    private void ShowCurrentStep()
    {
        _tutorialUI.ShowStep(CurrentStep);

        if (!_hasSpawnedTutorialEnemies && _waveEnemySpawner != null &&
            CurrentStep == TutorialStep.Attack)
        {
            _hasSpawnedTutorialEnemies = true;
            _waveEnemySpawner.SpawnImmediateFromCurrentWave(_enemySpawnCount);
        }

        if (CurrentStep == TutorialStep.Boss && !_hasSpawnedBoss)
        {
            _hasSpawnedBoss = true;

            if (_waveEnemySpawner == null || _bossPrefab == null)
            {
                Debug.LogError("TutorialManager requires a WaveEnemySpawner and Boss Prefab for the boss step.");
                return;
            }

            _bossHealth = _waveEnemySpawner.SpawnImmediate(_bossPrefab);
            if (_bossHealth != null)
                _bossHealth.OnDied += CompleteBossBattle;
        }

        if (CurrentStep == TutorialStep.Complete)
        {
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            PlayerPrefs.Save();

            if (GameManager.Instance != null)
                GameManager.Instance.GameClear();
            else
                Debug.LogError("Tutorial clear requires a GameManager in the scene.");
        }
    }
}
