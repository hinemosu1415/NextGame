using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StructureBuilding/StructureDatabase")]
public class StructureDatabase : ScriptableObject
{
    public List<StructureData> Database;
}
