using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; set;}
    public LevelLoader levelManager;
    public MenuManager MenuManager; 

    public HUD hud;
    public Cycle cycle;

    private void Awake()
    {
        if(Instance != null && Instance !=this)
        {
            Destroy(this);
        }

        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(levelManager.gameObject);
    }

    public void Update()
    {
        hud.UpdateTimerText(cycle.GetTimer());
    }

    
}
