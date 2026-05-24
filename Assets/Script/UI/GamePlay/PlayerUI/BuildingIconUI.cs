using UnityEngine;

public class BuildingIconUI : MonoBehaviour
{
    [SerializeField] private GameObject _coinIcon;
    [SerializeField] private GameObject _groundIcon;

    private void Start()
    {
        _coinIcon.SetActive(false);
        _groundIcon.SetActive(false);
    }

    public void UpdateState(BuildCheckResult checkResult)
    {
        SetCoinIconActive(!checkResult.StructureEntry.CanAfford);
        SetGroundIconActive(!checkResult.IsPlacementValid);
    }

    private void SetCoinIconActive(bool active)
    {
        _coinIcon.SetActive(active);
    }

    private void SetGroundIconActive(bool active)
    {
        _groundIcon.SetActive(active);
    }
}
