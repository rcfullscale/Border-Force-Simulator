using UnityEngine;

public class BoatSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] boatPrefabs;           // Drag in one or more boat prefabs
    public int boatCount = 10;                 // How many boats to spawn
    public float spawnRadius = 15f;            // Area around this object to spawn within
    public float minSpacingBetweenBoats = 2f;  // Minimum gap between spawned boats

    [Header("Randomize Scale")]
    public bool randomizeScale = true;
    public float minScale = 0.8f;
    public float maxScale = 1.3f;

    [Header("Randomize Speed")]
    public bool randomizeSpeed = true;
    public float minSpeed = 1.0f;
    public float maxSpeed = 2.5f;

    private void Start()
    {
        SpawnBoats();
    }

    void SpawnBoats()
    {
        if (boatPrefabs == null || boatPrefabs.Length == 0)
        {
            Debug.LogWarning("BoatSpawner: No boat prefabs assigned!");
            return;
        }

        int spawned = 0;
        int maxAttempts = boatCount * 20; // Safety cap to avoid infinite loop
        int attempts = 0;

        // Track positions of already-spawned boats to enforce spacing
        Vector2[] spawnedPositions = new Vector2[boatCount];

        while (spawned < boatCount && attempts < maxAttempts)
        {
            attempts++;

            // Pick a random point within the spawn radius
            Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

            // Check it's not too close to an already-spawned boat
            bool tooClose = false;
            for (int i = 0; i < spawned; i++)
            {
                if (Vector2.Distance(randomPoint, spawnedPositions[i]) < minSpacingBetweenBoats)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            // Pick a random prefab from the array
            GameObject prefab = boatPrefabs[Random.Range(0, boatPrefabs.Length)];

            // Spawn with a random rotation
            float randomAngle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, randomAngle);
            Vector3 spawnPos = new Vector3(randomPoint.x, randomPoint.y, -1f);
            GameObject boat = Instantiate(prefab, spawnPos, rotation, transform);

            // Optionally randomize scale
            if (randomizeScale)
            {
                float scale = 0.5f;
                    boat.transform.localScale = Vector3.one * scale;
            }

            // Optionally randomize speed on the movement script
            if (randomizeSpeed)
            {
                BoatRandomMovement2D movement = boat.GetComponent<BoatRandomMovement2D>();
                if (movement != null)
                    movement.maxSpeed = Random.Range(minSpeed, maxSpeed);
            }

            boat.name = $"Boat_{spawned + 1}";
            spawnedPositions[spawned] = randomPoint;
            spawned++;
        }

        if (spawned < boatCount)
            Debug.LogWarning($"BoatSpawner: Only managed to spawn {spawned}/{boatCount} boats. Try increasing spawnRadius or decreasing minSpacingBetweenBoats.");
        else
            Debug.Log($"BoatSpawner: Spawned {spawned} boats.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
        DrawCircle2D(transform.position, spawnRadius);
    }

    void DrawCircle2D(Vector2 center, float radius)
    {
        int segments = 40;
        float step = 360f / segments * Mathf.Deg2Rad;
        for (int i = 0; i < segments; i++)
        {
            Vector3 a = (Vector3)center + new Vector3(Mathf.Cos(i * step) * radius, Mathf.Sin(i * step) * radius);
            Vector3 b = (Vector3)center + new Vector3(Mathf.Cos((i + 1) * step) * radius, Mathf.Sin((i + 1) * step) * radius);
            Gizmos.DrawLine(a, b);
        }
    }
}
