using UnityEngine;
using TMPro;

public class CurrencyDisplayUI : MonoBehaviour
{
    [SerializeField] private CurrencyWallet _playerWallet;
    [SerializeField] private TMP_Text _numberText;
    private void Start()
    {
        // 初期表示
        int current = _playerWallet.GetCurrencyAmount(CurrencyData.CurrencyType.Coin);
        _numberText.text = current.ToString();

        // イベント購読
        _playerWallet.OnCurrencyChanged += UpdateUI;
    }

    private void OnDestroy()
    {
        // イベント購読解除（必須）
        _playerWallet.OnCurrencyChanged -= UpdateUI;
    }

    private void UpdateUI(CurrencyData.CurrencyType type, int amount)
    {
        // Coin の時だけ UI を更新
        if (type == CurrencyData.CurrencyType.Coin)
        {
            _numberText.text = amount.ToString();
        }
    }
}

