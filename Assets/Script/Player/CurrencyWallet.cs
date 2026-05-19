using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CurrencyData
{
    public enum CurrencyType
    {
        Coin,
        Gem,
        Experience
    }

    public CurrencyType Type;
    public int MaxAmount;
    public int Amount;
}

public class CurrencyWallet : MonoBehaviour
{
    [SerializeField] private List<CurrencyData> _currencies;

    public event Action<CurrencyData.CurrencyType, int> OnCurrencyChanged;

    public int GetCurrencyAmount(CurrencyData.CurrencyType type)
    {
        CurrencyData data = GetCurrencyData(type);
        if (data != null)
        {
            return data.Amount;
        }
        return 0;
    }

    public void AddCurrency(CurrencyData.CurrencyType type, int amount)
    {
        CurrencyData data = GetCurrencyData(type);
        if (data != null)
        {
            data.Amount = Mathf.Clamp(data.Amount + amount, 0, data.MaxAmount);
            OnCurrencyChanged?.Invoke(type, data.Amount);
        }
    }

    public bool TryConsumeCurrency(CurrencyData.CurrencyType type, int amount)
    {
        CurrencyData data = GetCurrencyData(type);
        if (data != null && data.Amount >= amount)
        {
            data.Amount -= amount;
            OnCurrencyChanged?.Invoke(type, data.Amount);
            return true;
        }
        return false;
    }

    private CurrencyData GetCurrencyData(CurrencyData.CurrencyType type)
    {
        return _currencies.FirstOrDefault(c => c.Type == type);
    }
}
