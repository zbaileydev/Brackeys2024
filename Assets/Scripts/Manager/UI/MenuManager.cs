using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("SFX")]
    public AudioSource hoverSound;
    public AudioSource sliderSound;
    public AudioSource clickSound;

    [Header("Menu Canvas")]
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public GameObject mainMenu;
    public GameObject HUDMenu;

    [Header("Transitions")]
    public Animator animator;
    public Animator pauseAnimator;

    [Header("Managers")]
    public LevelLoader levelLoader;

    private GameObject[] panels;
    private bool isPaused = false;
    //private FirstPersonController fpsController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {
        panels = new GameObject[] {optionsMenu, creditsMenu, mainMenu, pauseMenu};
    }

    // Toggling all panels besides the one passed in.
    public void PanelSwitch(GameObject activePanel, bool hudCondition = true)
    {
        foreach (var panel in panels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
            
        }
        // Disable all panels for the loading screen
        // before we render the HUD.
        if (!hudCondition) activePanel.SetActive(true);
    }

    // Determine if we are returning to the main menu 
    // or the pause menu.
    public void BackButton()
    {

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0)
        {
            PanelSwitch(mainMenu);
        }
        else
        {
            PanelSwitch(pauseMenu);
        }
    }

    public void QuitGame()
    {
        StartCoroutine(ExitProcess());
    }

    IEnumerator ExitProcess()
    {
        yield return new WaitForSeconds(3);

        #if UNITY_EDITOR
        // If in the editor, stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // If in a build, quit the application
        Application.Quit();
        #endif

        yield return null;

        
    }

    // SFX for interacting with UI elements.
    /*
    public void PlayHover(){
		hoverSound.Play();
	}

    public void PlaySFXHover(){
        sliderSound.Play();
    }

    public void PlayClick(){
        clickSound.Play();
    }
    */

    void Update()
    {
        // Only run on levels besides the main menu.
        if (Input.GetKeyDown(KeyCode.L) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                //PauseGame();
            }
        }
    }

    /*
    public void PauseGame()
    {
        isPaused = true;
        fpsController = FindObjectOfType<FirstPersonController>();

        if (fpsController != null) fpsController.enabled = false;
        pauseMenu.SetActive(true);
       
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseAnimator.Play("menuOpen");
        Time.timeScale = 0f;
       
        Debug.Log("Timescale is: " + Time.timeScale);
    }
    */

    public void ResumeGame()
    {
        Debug.Log("Resuming");
        StartCoroutine(ResumeGameAnimation());
    }

    IEnumerator ResumeGameAnimation()
    {
        //pauseAnimator.Play("menuClose");
        Time.timeScale = 1f;
        //pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        //if (fpsController != null) fpsController.enabled = true;
        yield return null;
    }

    public void ClickPlay()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        levelLoader.LoadScene(nextSceneIndex);
        PanelSwitch(HUDMenu, false);
        StartCoroutine(StartHUD());
    }

    IEnumerator StartHUD()
    {
        // The main issue is beach generation takes multiple seconds
        // so waiting for the level to load reduces the calm period
        // need to add an event for when generation finishes.
        while (levelLoader == null)
        {
            yield return new WaitForSeconds(1f);
        }

        while (!levelLoader.GetLoadingStatus())
        {
            yield return new WaitForSeconds(1f);
        }
        
        PanelSwitch(HUDMenu);
    }

}
