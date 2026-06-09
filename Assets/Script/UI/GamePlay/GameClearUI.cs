using UnityEngine;

public class GameClearUI : MonoBehaviour
{
    [SerializeField] private GameObject _gameClearPanel;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        _gameClearPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.GameClear)
        {
            _gameClearPanel.SetActive(true);
        }
        else
        {
            _gameClearPanel.SetActive(false);
        }
    }
}
