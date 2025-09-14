using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Tooltip("Possible cloud prefabs to spawn.")]
    public CloudMover[] cloudPrefabs;

    [Tooltip("X position where clouds spawn.")]
    public float spawnX = 50f;

    [Tooltip("Range of Y positions for spawning.")]
    public Vector2 spawnYRange = new Vector2(10f, 20f);

    [Tooltip("Range in seconds between spawns.")]
    public Vector2 spawnIntervalRange = new Vector2(3f, 7f);

    [Tooltip("Range of drift speeds assigned to clouds.")]
    public Vector2 speedRange = new Vector2(1f, 3f);

    private float nextSpawn;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        nextSpawn -= Time.deltaTime;
        if (nextSpawn <= 0f)
        {
            Spawn();
            ScheduleNextSpawn();
        }
    }

    private void Spawn()
    {
        if (cloudPrefabs == null || cloudPrefabs.Length == 0) return;

        var prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        float y = Random.Range(spawnYRange.x, spawnYRange.y);
        var cloud = Instantiate(prefab, new Vector3(spawnX, y, 0f), Quaternion.identity, transform);
        cloud.speed = Random.Range(speedRange.x, speedRange.y);
    }

    private void ScheduleNextSpawn()
    {
        nextSpawn = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
    }
}
