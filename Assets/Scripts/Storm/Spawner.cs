using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro
using System;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab here
    public float spawnRadius = 20f; // Radius around the player for spawning enemies
    public float timeBetweenWaves = 5f; // Time between each wave
    public int enemiesPerWave = 5; // Number of enemies per wave
    public float timeBetweenSpawns = 1f; // Time between enemy spawns within a wave
    public TextMeshProUGUI waveTimerText; // UI to display countdown to the next wave
    public LayerMask collisionMask; // Define layers to check for collisions
    public int maxEnemiesPerWave = 50; // Set Maximum Enemies per wave

    private float countdown; // Tracking timer for next waves
    private int waveNumber = 1;
    private Transform player; // Reference to the player's transform
    private int maxSpawnAttempts = 10; // Maximum number of attempts to find a legal spawn position

    private void Start()
    {
        enemiesPerWave = Mathf.Min(enemiesPerWave, maxEnemiesPerWave); // Checks Max wave spanwer
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object by tag
        countdown = timeBetweenWaves; // Set countdown to timeBetweenWaves
        StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            countdown = 0; // Keeps timer on zero while enemies spawn
        }

        waveTimerText.text = "Next Wave In: " + Mathf.Ceil(countdown).ToString() + "s"; // Update UI text
    }

    IEnumerator SpawnWave()
    {
        while (true)
        {
            countdown = timeBetweenWaves; // Reset countdown before new waves

            yield return new WaitForSeconds(timeBetweenWaves); // Wait for the next wave
            StartCoroutine(SpawnEnemies(waveNumber)); // Start spawning enemies for the wave
            //waveNumber++; // Increment wave number

            enemiesPerWave += 1; // Increase the number of enemies per wave
            //timeBetweenWaves = Mathf.Max(1f, timeBetweenWaves - 0.5f); // Decrease time between waves
        }
    }

    IEnumerator SpawnEnemies(int waveSize)
    {
        for (int i = 0; i < waveSize * enemiesPerWave; i++)
        {
            SpawnEnemyNearPlayer(); // Spawn enemy near the player
            yield return new WaitForSeconds(timeBetweenSpawns); // Wait before spawning the next enemy
        }
    }

    private void SpawnEnemyNearPlayer()
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
            /*Rigidbody2D rb = newEnemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Apply a downward force
                rb.AddForce(new UnityEngine.Vector2(0, -10f), ForceMode2D.Impulse); // Adjust force as needed
                UnityEngine.Debug.Log("Force Applied");
            }
            */
            UnityEngine.Debug.Log("Enemy spawned at: " + spawnPosition);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Could not find a valid spawn location after multiple attempts.");
        }
    }
}