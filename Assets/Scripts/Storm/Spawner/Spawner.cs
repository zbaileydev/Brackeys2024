using System.Collections;
using UnityEngine;
using TMPro; //TextMeshPro

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy prefab here

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
        //if(Input.GetKeyDown(KeyCode.O)) SpawnEnemy();
        //waveTimerText.text = "Next Wave In: " + Mathf.Ceil(countdown).ToString() + "s"; //UI text every update
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
        Vector2 initialPoint = (Vector2)GameManager.Instance.player.transform.position + new Vector2((Random.Range(0,2) == 0 ? -1 : 1)*12, GameManager.Instance.player.transform.position.y);
        bool pointIsValid = false;
        while(!pointIsValid)
        {
            if(!Physics2D.Raycast(initialPoint,Vector2.zero,1, LayerMask.GetMask("Environment")) && Physics2D.Raycast(new Vector2(initialPoint.x,initialPoint.y-1),Vector2.zero,1, LayerMask.GetMask("Environment")))
                pointIsValid = true;
            else if(Physics2D.Raycast(new Vector2(initialPoint.x,initialPoint.y+1),Vector2.zero,1, LayerMask.GetMask("Environment"))) initialPoint.y +=1;
            else initialPoint.y -=1;
        }
        Instantiate(enemyPrefab, initialPoint, Quaternion.identity);
    }
}