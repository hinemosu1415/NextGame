using System;
using UnityEngine;

public enum TutorialStep
{
    Move,
    Jump,
    HealthExplanation,
    WaveExplanation,
    AttackExplanation,
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
    [SerializeField] private TutorialUI _tutorialUI;
    [SerializeField] private float _requiredMoveDistance = 3f;
    [Header("Tutorial enemy wave")]
    [SerializeField] private WaveEnemySpawner _waveEnemySpawner;

    public TutorialStep CurrentStep { get; private set; } = TutorialStep.Move;

    private float _startX;
    private Health _practiceEnemyHealth;
    private bool _hasSpawnedTutorialEnemies;
    private Health _bossHealth;
    private bool _hasSpawnedBoss;

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
            _waveEnemySpawner.SetAutoSpawn(false);
        _player.OnJumped += CompleteJump;
        _player.OnModeChanged += CompleteModeChange;
        if (_buildingManager != null) _buildingManager.OnStructurePlaced += CompleteStructurePlacement;
        if (_allyManager != null) _allyManager.OnAllySpawned += CompleteAllySummon;
        _tutorialUI.OnAdvanceRequested += CompleteManualStep;
        if (_waveEnemySpawner != null)
            _waveEnemySpawner.OnEnemySpawned += HandleEnemySpawned;

        ShowCurrentStep();
    }

    private void Update()
    {
        if (CurrentStep == TutorialStep.Move)
            CheckMoveProgress();
    }

    private void CheckMoveProgress()
    {
        if (Mathf.Abs(_player.transform.position.x - _startX) >= _requiredMoveDistance)
            AdvanceTo(TutorialStep.Jump);
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnJumped -= CompleteJump;
            _player.OnModeChanged -= CompleteModeChange;
        }
        if (_buildingManager != null) _buildingManager.OnStructurePlaced -= CompleteStructurePlacement;
        if (_allyManager != null) _allyManager.OnAllySpawned -= CompleteAllySummon;
        if (_tutorialUI != null) _tutorialUI.OnAdvanceRequested -= CompleteManualStep;
        if (_practiceEnemyHealth != null) _practiceEnemyHealth.OnDied -= CompletePracticeEnemyBattle;
        if (_waveEnemySpawner != null) _waveEnemySpawner.OnEnemySpawned -= HandleEnemySpawned;
        if (_bossHealth != null) _bossHealth.OnDied -= CompleteBossBattle;
    }

    private void CompleteJump()
    {
        if (CurrentStep == TutorialStep.Jump) AdvanceTo(TutorialStep.HealthExplanation);
    }

    private void CompleteManualStep()
    {
        if (CurrentStep == TutorialStep.HealthExplanation)
            AdvanceTo(TutorialStep.WaveExplanation);
        else if (CurrentStep == TutorialStep.WaveExplanation)
            AdvanceTo(TutorialStep.AttackExplanation);
        else if (CurrentStep == TutorialStep.AttackExplanation)
            AdvanceTo(TutorialStep.Attack);
        else if (CurrentStep == TutorialStep.SummonAlly)
            AdvanceTo(TutorialStep.Boss);
    }

    private void CompletePracticeEnemyBattle()
    {
        if (_practiceEnemyHealth != null)
            _practiceEnemyHealth.OnDied -= CompletePracticeEnemyBattle;

        if (CurrentStep == TutorialStep.Attack)
            AdvanceTo(TutorialStep.ChangeMode);
    }

    private void SubscribeToAttackTarget(Health enemyHealth)
    {
        if (enemyHealth == null) return;
        if (_practiceEnemyHealth != null)
            _practiceEnemyHealth.OnDied -= CompletePracticeEnemyBattle;

        _practiceEnemyHealth = enemyHealth;
        _practiceEnemyHealth.OnDied += CompletePracticeEnemyBattle;
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
        if ((int)nextStep <= (int)CurrentStep) return;

        CurrentStep = nextStep;
        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        _tutorialUI.ShowStep(CurrentStep);

        if (!_hasSpawnedTutorialEnemies && _waveEnemySpawner != null &&
            CurrentStep == TutorialStep.Attack)
        {
            _hasSpawnedTutorialEnemies = true;
            _waveEnemySpawner.SpawnNextEnemy();
        }

        if (CurrentStep == TutorialStep.Boss && !_hasSpawnedBoss)
        {
            _hasSpawnedBoss = true;

            _bossHealth = _waveEnemySpawner.SpawnNextEnemy();
            if (_bossHealth == null)
            {
                Debug.LogError("The tutorial WaveData requires a boss after the practice enemy.");
                return;
            }

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
