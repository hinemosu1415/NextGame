using System.Collections;
using UnityEngine;
using Utility;

public class CoinGenerator : MonoBehaviour
{
    [SerializeField] private int _coinAmount;
    [SerializeField] private float _interval;

    private CurrencyWallet _wallet;
    private CountdownTimer _timer;

    private void Awake()
    {
        _wallet = GetComponent<CurrencyWallet>();
    }

    private void Start()
    {
        _timer = new CountdownTimer(_interval);
        _timer.Start();
    }

    private void Update()
    {
        _timer.Tick();

        if (_timer.IsFinished())
        {
            _wallet.AddCurrency(CurrencyData.CurrencyType.Coin, _coinAmount);
            _timer.Start();
        }
    }
}