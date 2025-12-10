using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] public int treeCount = 100;

    [Header("Terrain area")]
    [SerializeField] private Terrain terrain;         

    [Header("No-spawn zone")]
    [SerializeField] private Transform forbiddenCenter;
    [SerializeField] private float forbiddenRadius = 20f;

    void Start()
    {

        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        for (int i = 0; i < treeCount; i++)
        {
            float randX = Random.Range(0f, data.size.x);
            float randZ = Random.Range(0f, data.size.z);

            Vector3 worldPos = new Vector3(
                terrainPos.x + randX,
                0f,
                terrainPos.z + randZ
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

            float y = terrain.SampleHeight(worldPos) + terrainPos.y;
            worldPos.y = y;

            Instantiate(treePrefab, worldPos, Quaternion.identity, transform);
        }
    }
}
