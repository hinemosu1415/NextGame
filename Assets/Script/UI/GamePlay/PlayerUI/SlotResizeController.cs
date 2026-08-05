using UnityEngine;

public abstract class SlotUI : MonoBehaviour
{
    [SerializeField] protected GameObject _slotContents;

    public abstract void Focused();
    public abstract void Unfocused();
}

public class SlotResizeController : MonoBehaviour
{
    [SerializeField] private SlotUI _weaponSlot;
    [SerializeField] private SlotUI _structureSlot;
    [SerializeField] private Vector2 _unfocusScale;
    private Vector3 _defaultScale;

    private PlayerController _playerController;

    public void Init(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.OnModeChanged += UpdateSlotSize;
    }

    private void Start()
    {
        _defaultScale = _weaponSlot.transform.localScale;
        UpdateSlotSize(_playerController.CurrentMode);
    }

    private void UpdateSlotSize(PlayerController.Mode mode)
    {
        if (mode == PlayerController.Mode.Attack)
        {
            _weaponSlot.transform.localScale = _defaultScale;
            _weaponSlot.Focused();
            _structureSlot.transform.localScale = _unfocusScale;
            _structureSlot.Unfocused();
        }
        else
        {
            _weaponSlot.transform.localScale = _unfocusScale;
            _weaponSlot.Unfocused();
            _structureSlot.transform.localScale = _defaultScale;
            _structureSlot.Focused();
        }
    }
}
