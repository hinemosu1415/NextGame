using UnityEngine;

public class Coin : Item
{
    [SerializeField] private int _cost = 1;
    protected override void Execute(GameObject target)
    {
        // コインを獲得する処理
        if (target.TryGetComponent(out CurrencyWallet wallet))
        {
            wallet.AddCurrency(CurrencyData.CurrencyType.Coin, _cost);
        }
    }
}
