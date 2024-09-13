using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    public LevelLoader levelManager;
    public MenuManager MenuManager;
    // public Tilemap groundTilemap;
    public HUD hud;
    public Cycle cycle;
    [HideInInspector]
    public GameObject player;

    [SerializeField]
    GameObject playerPrefab;

    private bool startedGame;
    private bool gamePhase;
    private bool initial;

    /*
    Game Manager flow
    MenuManager calls levelManager when Play is pressed.
    When the demo scene is loaded in, MenuManager activates the HUD.
    Once the HUD is active, GameManager starts the game
    and turns on the CycleManager.

    GM then facilitates updating the HUD based on the CycleManager.

    We will likely rename CycleManager to StormManager
    and have it manage an EnemySpawner and other affects
    so that GM only has to worry about the high level state of the game.
    */

    private void Start()
    {
        initial = false;
        startedGame = false;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(levelManager.gameObject);
        DontDestroyOnLoad(MenuManager.gameObject);
        DontDestroyOnLoad(cycle.gameObject);
    }

    public void Update()
    {
        if (startedGame && SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameCycle();
        }

        // If our HUD is enabled, we are in the game
        // so kick off the game cycle and the loop.
        if (hud.gameObject.activeInHierarchy == true)
        {
            startedGame = true;
            initial = true;
        }

        if (initial && SceneManager.GetActiveScene().buildIndex == 1)
        {
            player = FindObjectOfType<Player>().gameObject;
        }
    }

    void GameCycle()
    {
        if (cycle.gameObject.activeInHierarchy == false)
        {
            cycle.gameObject.SetActive(true);
        }

        hud.UpdateTimerText(cycle.GetTimer());

        if (cycle.GetTimer() < 2f)
        {
            gamePhase = cycle.GetCalmPhase();
            initial = true;
        }

        if (gamePhase && initial)
        {
            Debug.Log("Calm");
            initial = false;
        }
        else if (initial)
        {
            Debug.Log("Storm");
            initial = false;
        }
    }

    // Semirose: Not sure if this belongs here
    public void SpawnPlayer(Vector3 pos)
    {
        var players = FindObjectsOfType<PlayerMovement>();
        if (players.Length != 0)
            foreach (var player in players)
                Destroy(player.gameObject);

        GameObject newPlayer = Instantiate(playerPrefab, pos, Quaternion.identity);
        player = newPlayer;
        Camera.main.GetComponent<CameraFollow>().target = player.transform;
    }
}
