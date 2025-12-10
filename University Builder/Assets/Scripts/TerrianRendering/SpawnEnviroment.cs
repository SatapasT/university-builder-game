using UnityEngine;

public class SpawnEnvironment : MonoBehaviour
{
    [System.Serializable]
    public class SpawnConfig
    {
        public string name;
        public GameObject prefab;
        public int count = 100;
    }

    [Header("Things to spawn")]
    [SerializeField] private SpawnConfig[] spawnConfigs;

    [Header("Terrain area")]
    [SerializeField] private Terrain terrain;

    [Header("No-spawn zone")]
    [SerializeField] private Transform forbiddenCenter;
    [SerializeField] private float forbiddenRadius = 20f;

    private TerrainData terrainData;
    private Vector3 terrainOrigin;

    private void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("SpawnEnvironment: Terrain reference is missing!");
            return;
        }

        terrainData = terrain.terrainData;
        terrainOrigin = terrain.transform.position;

        foreach (var config in spawnConfigs)
        {
            if (config.prefab == null || config.count <= 0)
                continue;

            SpawnObjects(config);
        }
    }

    private void SpawnObjects(SpawnConfig config)
    {
        for (int i = 0; i < config.count; i++)
        {
            float randX = Random.Range(0f, terrainData.size.x);
            float randZ = Random.Range(0f, terrainData.size.z);

            Vector3 worldPos = new Vector3(
                terrainOrigin.x + randX,
                0f,
                terrainOrigin.z + randZ
            );

            if (forbiddenCenter != null)
            {
                float dist = Vector3.Distance(
                    new Vector3(worldPos.x, forbiddenCenter.position.y, worldPos.z),
                    forbiddenCenter.position
                );

                if (dist < forbiddenRadius)
                {
                    i--;   
                    continue;
                }
            }

            float y = terrain.SampleHeight(worldPos) + terrainOrigin.y;
            worldPos.y = y;

            Instantiate(config.prefab, worldPos, Quaternion.identity, transform);
        }
    }
}
