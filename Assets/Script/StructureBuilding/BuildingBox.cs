using UnityEngine;
using Utility;

public class BuildingBox : MonoBehaviour
{
    [SerializeField] private AudioClip _buildingSound;
    [SerializeField] private AudioClip _buildCompleteSound;

    private GameObject _structurePrefab;
    private GameObject _player;
    private StructureData _structureData;
    private CountdownTimer _buildTimer;
    private Health _health;
    protected void Awake()
    {
        _health = GetComponent<Health>();
    }
    protected void Start()
    {
        if (_health != null)
        {
            _health.OnDied += Die;
        }

        const float MIN_BUILD_TIME = 1f;
        if (_structureData.BuildTime > MIN_BUILD_TIME)
        {
            AudioManager.Instance.PlaySoundEffect(_buildingSound);
        }
    }

    public void Init(StructureData data, GameObject player)
    {
        _structureData = data;
        _structurePrefab = data.Prefab;
        _player = player;
        _buildTimer = new CountdownTimer(data.BuildTime);
        _buildTimer.Start();
        transform.localScale = new Vector2(
            transform.localScale.x * data.GridSize.x,
            transform.localScale.y * data.GridSize.y
        );
    }

    private void Update()
    {
        _buildTimer.Tick();
        if (_buildTimer.IsFinished())
        {
            Structure structure = Instantiate(_structurePrefab, transform.position, Quaternion.identity).GetComponent<Structure>();
            structure.Init(_player);
            AudioManager.Instance.PlaySoundEffect(_buildCompleteSound);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDied -= Die;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}