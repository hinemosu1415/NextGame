using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Playing,
    GameOver,
    GameClear
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; } = GameState.Playing;
    public Action<GameState> OnGameStateChanged;
    public List<IWaveProgressProvider> EnemyWaveProgressProviders { get; private set; } = new(); //敵スポナーが複数あっても対応できるようにリストで管理
    public float EnemyWaveProgress { get; private set; } = 0;
    public event Action<float> OnEnemyWaveProgressChanged;



    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate()
    {
        float newProgress = CalculateEnemyWaveProgress();
        const float PROGRESS_THRESHOLD = 0.001f;
        if (Math.Abs(newProgress - EnemyWaveProgress) <= PROGRESS_THRESHOLD) return;

        EnemyWaveProgress = newProgress;
        OnEnemyWaveProgressChanged?.Invoke(EnemyWaveProgress);

        if (newProgress >= 1f)
        {
            GameClear();
        }
    }

    private float CalculateEnemyWaveProgress()
    {
        if (EnemyWaveProgressProviders.Count == 0) return 0;
        float totalProgress = 0;
        foreach (var provider in EnemyWaveProgressProviders)
        {
            totalProgress += provider.ProgressRatio;
        }
        return totalProgress / EnemyWaveProgressProviders.Count;
    }

    public void OnRocketDestroyed()
    {
        GameOver();
    }

    private void GameOver()
    {
        SetGameState(GameState.GameOver);
        Time.timeScale = 0f;
    }

    private void GameClear()
    {
        SetGameState(GameState.GameClear);
    }

    private void SetGameState(GameState newState)
    {
        if (CurrentGameState == newState) return;
        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(CurrentGameState);
    }
}