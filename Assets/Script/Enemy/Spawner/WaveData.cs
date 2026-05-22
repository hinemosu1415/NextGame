using UnityEngine;

[CreateAssetMenu(menuName = "Wave/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject EnemyPrefab;
        public float SpawnDelaySecond; // 出現までの時間
    }

    public EnemySpawnInfo[] EnemyList;
}