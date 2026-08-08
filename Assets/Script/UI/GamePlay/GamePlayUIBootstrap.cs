using UnityEngine;

public class GamePlayUIBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private AllyUI _allyUI;
    [SerializeField] private StructureSlotUI _structureSlotUI;
    [SerializeField] private SlotResizeController _slotResizeController;
    [SerializeField] private PlayerWeaponSlotUI _playerWeaponSlotUI;
    [SerializeField] private CurrencyDisplayUI _currencyDisplayUI;
    [SerializeField] private HealthBarCanvas _playerHealthBar;

    private void Awake()
    {
        _allyUI.Init(_player.GetComponent<PlayerAllyManager>());
        _slotResizeController.Init(_player.GetComponent<PlayerController>());
        _structureSlotUI.Init(_player.GetComponent<PlayerBuildingManager>());
        _playerWeaponSlotUI.Init(_player.GetComponent<PlayerWeaponManager>());
        _currencyDisplayUI.Init(_player.GetComponent<CurrencyWallet>());
        _playerHealthBar.Init(_player.GetComponent<Health>());
    }
}
