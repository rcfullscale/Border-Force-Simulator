using UnityEngine;

public class BoatSpawner : MonoBehaviour
{
    [Header("Normal Boats")]
    public GameObject[] boatPrefabs;           // Normal boat prefabs
    public int boatCount = 10;

    [Header("Rare Bad Boat")]
    public GameObject badBoatPrefab;           // The always-invalid boat prefab
    [Range(0f, 1f)]
    public float badBoatChance = 0.15f;        // 15% chance each spawn is a bad boat

    [Header("Spawn Settings")]
    public float spawnRadius = 15f;
    public float minSpacingBetweenBoats = 2f;

    [Header("Randomize Scale")]
    public bool randomizeScale = true;
    public float minScale = 0.8f;
    public float maxScale = 1.3f;

    [Header("Randomize Speed")]
    public bool randomizeSpeed = true;
    public float minSpeed = 1.0f;
    public float maxSpeed = 2.5f;

    void Start()
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
        int maxAttempts = boatCount * 20;
        int attempts = 0;
        Vector2[] spawnedPositions = new Vector2[boatCount];

        while (spawned < boatCount && attempts < maxAttempts)
        {
            attempts++;

            Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

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

            // Decide if this is a bad boat
            bool isBadBoat = badBoatPrefab != null && Random.value < badBoatChance;
            GameObject prefab = isBadBoat
                ? badBoatPrefab
                : boatPrefabs[Random.Range(0, boatPrefabs.Length)];

            float randomAngle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, randomAngle);
            Vector3 spawnPos = new Vector3(randomPoint.x, randomPoint.y, -1f);
            GameObject boat = Instantiate(prefab, spawnPos, rotation, transform);

            // Mark bad boats as always invalid
            if (isBadBoat)
            {
                BoatData data = boat.GetComponent<BoatData>();
                if (data != null) data.alwaysDestroy = true;
            }

            if (randomizeScale)
                boat.transform.localScale = Vector3.one * 0.5f;

            if (randomizeSpeed)
            {
                BoatRandomMovement2D movement = boat.GetComponent<BoatRandomMovement2D>();
                if (movement != null)
                    movement.maxSpeed = Random.Range(minSpeed, maxSpeed);
            }

            boat.name = isBadBoat ? $"BadBoat_{spawned + 1}" : $"Boat_{spawned + 1}";
            spawnedPositions[spawned] = randomPoint;
            spawned++;
        }

        if (spawned < boatCount)
            Debug.LogWarning($"BoatSpawner: Only spawned {spawned}/{boatCount} boats.");
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