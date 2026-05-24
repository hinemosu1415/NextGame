using UnityEngine;
using TMPro;

public class CountdownDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countdownText;

    private void Awake()
    {
        if (_countdownText == null)
        {
            _countdownText = GetComponent<TextMeshProUGUI>();
        }

        if (_countdownText == null)
        {
            Debug.LogError("[CountdownDisplay] Text component is null!");
        }

        SetActive(false);
    }

    public void SetCountdown(float remainingTime)
    {
        if (_countdownText == null) return;

        int displaySeconds = Mathf.CeilToInt(remainingTime);
        _countdownText.text = displaySeconds.ToString();
    }

    public void SetActive(bool active)
    {
        if (_countdownText != null)
        {
            _countdownText.gameObject.SetActive(active);
        }
    }
}
