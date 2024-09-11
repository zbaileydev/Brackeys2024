using System.Collections;
using UnityEngine;
using TMPro; //TextMeshPro

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab here
    public Transform[] spawnPoints; // Multiple array spawn points
    public float timeBetweenWaves = 5f; // Time between each wave
    public int enemiesPerWave = 5; // Number of enemies per wave
    public float timeBetweenSpawns = 1f; // Time between enemy spawns within a wave
    public TextMeshProUGUI waveTimerText; //UI

    private float countdown; //tracking timer for next waves
    private int waveNumber = 1;

    private void Start()
    {
        //starts spawning event
        countdown = timeBetweenWaves; //sets countdown to timeBetweenwaves
        StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            countdown = 0; //keeps timer on zero while mobs spawn
        }

        waveTimerText.text = "Next Wave In: " + Mathf.Ceil(countdown).ToString() + "s"; //UI text every update
    }

    IEnumerator SpawnWave()
    {
        while (true)
        {
            countdown = timeBetweenWaves; //Reset Countdown before new waves

            //Variable of timeBetweenWaves will adjust the spawn timer
            yield return new WaitForSeconds(timeBetweenWaves);
            StartCoroutine(SpawnEnemies(waveNumber));
            waveNumber++;

            countdown = timeBetweenWaves;
            //adjusting diffculty, by adding enemiesPerWave += ##, timeBetweenWaves = Mathf.Max(1f, timeBetweenWaves - 0.5f);, 
        }
    }
    IEnumerator SpawnEnemies(int waveSize)
    {
        //This can be adjusted to increase difficulty per wave
        for (int i = 0; i < waveSize * enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    //spawns the enemy  (Prefab, can add multiple prefabs by adding more Prefab variables

    private void SpawnEnemy()
    {
        Debug.Log("ENEMY SPAWNED");
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
    }
}