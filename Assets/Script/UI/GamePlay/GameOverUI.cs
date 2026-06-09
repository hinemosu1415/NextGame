using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        _gameOverPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.GameOver)
        {
            _gameOverPanel.SetActive(true);
        }
        else
        {
            _gameOverPanel.SetActive(false);
        }
    }
}
