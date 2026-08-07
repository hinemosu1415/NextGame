using UnityEngine;

public class CoinFactory : Structure
{
    private CurrencyWallet _currencyWallet;

    public override void Init(GameObject player)
    {
        base.Init(player);
        _currencyWallet = player.GetComponent<CurrencyWallet>();
    }

    protected override void Execute()
    {
        _currencyWallet.AddCurrency(CurrencyData.CurrencyType.Coin, 1);
    }
}