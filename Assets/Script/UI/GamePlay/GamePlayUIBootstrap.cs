using UnityEngine;

public class GamePlayUIBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private AllyUI AllyUI;
    [SerializeField] private BuildingModeUI BuildingModeUI;
    [SerializeField] private CurrencyDisplayUI CurrencyDisplayUI;
    [SerializeField] private HealthBarCanvas PlayerHealthBar;

    private void Awake()
    {
        AllyUI.Init(Player.GetComponent<PlayerAllyManager>());
        BuildingModeUI.Init(Player.GetComponent<PlayerBuildingManager>(), Player.GetComponent<PlayerController>());
        CurrencyDisplayUI.Init(Player.GetComponent<CurrencyWallet>());
        PlayerHealthBar.Init(Player.GetComponent<Health>());
    }
}
