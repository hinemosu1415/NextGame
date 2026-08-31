using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Save/GameSaveData")]
public class GameSaveData : ScriptableObject
{
    [Header("Structures")]
    [SerializeField] private List<StructureData> _selectedStructures = new();

    public IReadOnlyList<StructureData> SelectedStructures => _selectedStructures;

    public void SetSelectedStructures(IEnumerable<StructureData> structures)
    {
        _selectedStructures.Clear();
        _selectedStructures.AddRange(structures);
    }

    public void SetSelectedStructure(int index, StructureData structure)
    {
        if (index < 0 || index >= _selectedStructures.Count)
        {
            return;
        }

        _selectedStructures[index] = structure;
    }
}