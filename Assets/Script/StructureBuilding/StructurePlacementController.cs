using UnityEngine;

public readonly struct BuildCheckResult
{
    public readonly bool IsPlacementValid;
    public readonly StructureEntry StructureEntry;
    public readonly bool CanBuild;

    public BuildCheckResult(bool isPlacementValid, StructureEntry structureEntry)
    {
        IsPlacementValid = isPlacementValid;
        StructureEntry = structureEntry;
        CanBuild = isPlacementValid && structureEntry.IsAvailable;
    }
}

public class StructurePlacementController : MonoBehaviour
{
    [SerializeField] private GameObject _placementPreviewObject;
    [SerializeField] private BuildingIconUI _iconUI;
    [SerializeField] private Vector2 _cellSize;
    [SerializeField] private BuildingBox _buildingBox;

    private StructurePreview _placementPreview;
    private StructurePlacementValidator _placementValidator;
    private GridSnapper _gridSnapper;
    private StructureEntry _structureEntry;
    public BuildCheckResult CurrentBuildCheck { get; private set; }

    private void Awake()
    {
        _placementPreview = _placementPreviewObject.GetComponent<StructurePreview>();
        _placementValidator = _placementPreviewObject.GetComponent<StructurePlacementValidator>();
        _gridSnapper = _placementPreviewObject.GetComponent<GridSnapper>();
        _gridSnapper.GridSetup(_cellSize);
    }

    private void Update()
    {
        if (_structureEntry == null) return;

        CurrentBuildCheck = new BuildCheckResult(_placementValidator.CanPlace(), _structureEntry);
        _iconUI.UpdateState(CurrentBuildCheck);
        _placementPreview.UpdateState(CurrentBuildCheck.CanBuild);
    }

    public void SetStructureEntry(StructureEntry entry)
    {
        _structureEntry = entry;
        _placementPreview.SetStructure(_structureEntry.StructureData);
        _gridSnapper.SetGridSize(_structureEntry.StructureData.GridSize);
    }

    public bool TryPlaceStructure(GameObject player)
    {
        //建造物の配置処理        
        if (CurrentBuildCheck.CanBuild)
        {
            BuildingBox buildingBox = Instantiate(_buildingBox, _placementPreview.transform.position, Quaternion.identity);
            buildingBox.Init(_structureEntry.StructureData, player);
            return true;
        }

        return false;
    }
}