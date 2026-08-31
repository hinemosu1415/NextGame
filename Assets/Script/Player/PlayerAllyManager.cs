using System;
using UnityEngine;
public class PlayerAllyManager : MonoBehaviour
{
    [SerializeField] private AllyData[] _allyEntries;
    [SerializeField] private int _spawnCost;
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private AudioClip _spawnSound;

    private CurrencyWallet _currencyWallet;
    private int _allyIndex = 0;

    public AllyData NextAllyEntry { get; private set; }
    public float ExperienceForNextRatio { get; private set; }
    public int AllyToken { get; private set; }
    public Action<float, int> OnSpawnCapacityChanged;
    public Action<AllyData> OnNextAllyEntryChanged;
    public event Action OnAllySpawned;

    private void Awake()
    {
        _currencyWallet = GetComponent<CurrencyWallet>();
        OnExperienceChanged(CurrencyData.CurrencyType.Experience, _currencyWallet.GetCurrencyAmount(CurrencyData.CurrencyType.Experience));
        NextAllyEntry = _allyEntries[_allyIndex];
    }

    private void Start()
    {
        _currencyWallet.OnCurrencyChanged += OnExperienceChanged;
    }

    private void OnDestroy()
    {
        _currencyWallet.OnCurrencyChanged -= OnExperienceChanged;
    }

    private void OnExperienceChanged(CurrencyData.CurrencyType type, int amount)
    {
        if (type == CurrencyData.CurrencyType.Experience)
        {
            int experienceForNext = amount % _spawnCost;
            ExperienceForNextRatio = (float)experienceForNext / _spawnCost;
            AllyToken = amount / _spawnCost;
            OnSpawnCapacityChanged?.Invoke(ExperienceForNextRatio, AllyToken);
        }
    }

    public bool TrySpawnAlly()
    {
        if (_currencyWallet.TryConsumeCurrency(CurrencyData.CurrencyType.Experience, _spawnCost))
        {
            GameObject ally = Instantiate(NextAllyEntry.Prefab, _spawnTransform.position, _spawnTransform.rotation);
            ally.GetComponent<CharacterAIController>().Init(_targetTransform);
            NextAllyEntry = GetNextAllyEntry();
            OnNextAllyEntryChanged?.Invoke(NextAllyEntry);
            OnAllySpawned?.Invoke();
            AudioManager.Instance.PlaySoundEffect(_spawnSound);
            return true;
        }

        return false;
    }

    private AllyData GetNextAllyEntry()
    {
        if (_allyIndex < _allyEntries.Length - 1) _allyIndex++;
        else _allyIndex = 0;

        return _allyEntries[_allyIndex];
    }
}
