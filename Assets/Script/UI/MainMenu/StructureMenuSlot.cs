using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructureMenuSlot : MonoBehaviour
{
    [SerializeField] GameObject _frame;
    [SerializeField] Image _image;
    [SerializeField] private TextMeshProUGUI _costText;
    public StructureData StructureData { get; private set; }
    public event Action<StructureMenuSlot> OnSelect;

    public void UpdateStructureData(StructureData data)
    {
        StructureData = data;
        _image.sprite = data.Image;
        _costText.text = data.Cost.ToString();
    }

    public void OnClick()
    {
        OnSelect?.Invoke(this);
    }

    public void ShowFrame()
    {
        _frame.SetActive(true);
    }

    public void HideFrame()
    {
        _frame.SetActive(false);
    }
}
