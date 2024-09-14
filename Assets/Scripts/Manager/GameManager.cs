using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public LevelLoader levelLoader;
    public MenuManager menuManager;
    public Settings settingsManager;
    public WorldGenerator worldGenerator;
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
        DontDestroyOnLoad(levelLoader.gameObject);
        DontDestroyOnLoad(menuManager.gameObject);
        DontDestroyOnLoad(cycle.gameObject);

        var p = FindObjectOfType<Player>();
        if (p != null)
            player = p.gameObject;
        settingsManager.LoadSettings();
    }

    public void Update()
    {
        if (startedGame)
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

        if (Input.GetMouseButtonDown(0))
        {
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void StartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = 1;
        levelLoader.LoadScene(nextSceneIndex);

        levelLoader.OnLevelLoaded += worldGenerator.StartGeneration;
    }

    public void ReplayGame()
    {
        cycle.Restart();
        startedGame = false;
        // No need for async or any magic, insta load the main menu.
        //SceneManager.LoadScene(0);
        SceneManager.LoadScene("Menu");
        //levelLoader.OnLevelLoaded += worldGenerator.StartGeneration;
    }

    void GameCycle()
    {
        if (cycle.gameObject.activeInHierarchy == false)
        {
            cycle.gameObject.SetActive(true);
        }

        if (hud.gameObject.activeInHierarchy == true)
        {
            hud.UpdateTimerText(cycle.GetTimer());
            if (player != null)
            {
                hud.UpdateHealthText(player.GetComponent<Player>().Health);
            }
        }

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

        if (player != null)
        {
            if (player.GetComponent<Player>().Health <= 0)
            {
                menuManager.GameOver();
            }
        }


    // @marcohamersma Feel free to move this to a more appropriate place
    void UpdateStormPresence()
    {
        /** A value from 0-1 representing how close the storm is. Used for audio
         * purposes  */
        var value = 1f;
        if (cycle.GetCalmPhase()) {
            value = 1 - (cycle.currentTime / cycle.calmTime);
        }

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("storm presence", value);
    }

    public void SpawnPlayer(Vector3 pos)
    {
        Debug.Log("Spawn started");
        var players = FindObjectsOfType<Player>();
        if (players.Length != 0)
            foreach (var player in players)
                Destroy(player.gameObject);
        
        // If either are null we need to delay...
        while (playerPrefab == null || pos == null)
        {
            Debug.Log(playerPrefab);
        }
        player = Instantiate(playerPrefab, pos, Quaternion.identity);

        Camera.main.GetComponent<CameraFollow>().target = player.transform;
    }
}
