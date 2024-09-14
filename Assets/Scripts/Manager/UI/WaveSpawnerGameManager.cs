using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnerGameManager : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // List of enemy prefabs
    public Spawner spawner; // Reference to the spawner
    public int enemiesToSpawn = 10; // Number of enemies to spawn during the storm phase
    public float spawnDelay = 1f; // Delay between spawns

    // Method to start the storm phase and spawn enemies
    public void StartStormPhase()
    {
        StartCoroutine(SpawnEnemiesDuringStorm());
    }

    // Coroutine to spawn enemies based on the enemy prefabs list
    private IEnumerator SpawnEnemiesDuringStorm()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Choose a random enemy prefab from the list
            GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Use the spawner to create the enemy
            spawner.SpawnEnemy(randomEnemy);

            // Optional: Delay between spawns
            yield return new WaitForSeconds(spawnDelay); // Adjust the spawn rate as needed
        }
    }
}