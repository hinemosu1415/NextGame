using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponEntryUI : MonoBehaviour
{
    [SerializeField] private Image _structureImage;
    [SerializeField] private GameObject _frame;
    [SerializeField] private TextMeshProUGUI _keyText;

    public void Init(PlayerWeaponData playerWeaponData, string keyName)
    {
        _structureImage.sprite = playerWeaponData.Image;
        _keyText.text = keyName;
    }

    public void SetSelected(bool isSelected)
    {
        _frame.SetActive(isSelected);
    }
}
