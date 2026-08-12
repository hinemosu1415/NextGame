using UnityEngine;

public class BGMController : MonoBehaviour
{
    [SerializeField] private AudioClip _stageBgmClip;
    [SerializeField] private AudioClip _gameClearBgmClip;
    [SerializeField] private AudioClip _gameOverBgmClip;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        AudioManager.Instance.PlayBGM(_stageBgmClip);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.GameOver)
        {
            if (_gameOverBgmClip == null)
            {
                AudioManager.Instance.StopBGM();
            }
            else
            {
                AudioManager.Instance.PlayBGM(_gameOverBgmClip);
            }
        }
        else if (gameState == GameState.GameClear)
        {
            if (_gameClearBgmClip == null)
            {
                AudioManager.Instance.StopBGM();
            }
            else
            {
                AudioManager.Instance.PlayBGM(_gameClearBgmClip);
            }
        }
    }
}
