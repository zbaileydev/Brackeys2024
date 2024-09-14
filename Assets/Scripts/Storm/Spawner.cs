using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab here
    public float spawnRadius = 20f; // Radius around the player for spawning enemies
    public LayerMask collisionMask; // Define layers to check for collisions
    private Transform player; // Reference to the player's transform
    private int maxSpawnAttempts = 10; // Maximum number of attempts to find a legal spawn position

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object by tag
    }

    public void SpawnEnemy(GameObject enemyPrefab)
    {
        UnityEngine.Vector3 spawnPosition = UnityEngine.Vector3.zero;
        bool validPositionFound = false;
        float enemyHeight = enemyPrefab.GetComponent<Collider2D>().bounds.size.y / 2; // Half height for accurate placement

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            UnityEngine.Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * spawnRadius;
            spawnPosition = new UnityEngine.Vector3(player.position.x + randomDirection.x, player.position.y + enemyHeight, 0);

            // Debug: Check spawn position
            UnityEngine.Debug.DrawLine(player.position, spawnPosition, UnityEngine.Color.red, 2f);

            if (!Physics2D.OverlapCircle(spawnPosition, enemyPrefab.transform.localScale.x / 2, collisionMask))
            {
                validPositionFound = true;
                break;
            }
        }

        if (validPositionFound)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, UnityEngine.Quaternion.identity);
            UnityEngine.Debug.Log("Enemy spawned at: " + spawnPosition);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Could not find a valid spawn location after multiple attempts.");
        }
    }
}
