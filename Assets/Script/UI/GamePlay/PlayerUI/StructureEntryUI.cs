using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructureEntryUI : MonoBehaviour
{
    [SerializeField] private Image _structureImage;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _keyText;
    [SerializeField] private GameObject _frame;
    [SerializeField] private GameObject _availableFill;

    private StructureEntry _entry;

    public void Init(StructureEntry entry, string keyName)
    {
        _entry = entry;
        _entry.OnIsAvailableChanged += UpdateAvailable;
        _structureImage.sprite = entry.StructureData.Image;
        _costText.text = entry.StructureData.Cost.ToString();
        _keyText.text = keyName;
    }

    public void OnDestroy()
    {
        _entry.OnIsAvailableChanged -= UpdateAvailable;
    }

    private void UpdateAvailable(bool isAvailable)
    {
        _availableFill.SetActive(!isAvailable);
    }

    public void SetSelected(bool isSelected)
    {
        _frame.SetActive(isSelected);
    }
}
