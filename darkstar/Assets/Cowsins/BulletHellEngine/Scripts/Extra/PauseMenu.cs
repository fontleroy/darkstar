using UnityEngine;
using cowsins.BulletHell; 

namespace cowsins.BulletHell
{

public class PauseMenu : MonoBehaviour
{
    private GameObject menu;

    public static PauseMenu pauseMenu;

    [SerializeField,Tooltip("Assign the order ID of your main menu in the Build Settings (File/BuildSettings)")] private int mainMenuOrder;

    // Returns true if the game is paused.
    public bool isPaused { get; private set; }
    private void Awake()
    {
        // Handle singleton
        if (pauseMenu == null) pauseMenu = this;

    }
    private void Start()
    {
        // Grab menu reference
        menu = transform.GetChild(0).gameObject;

        // Do not start the game paused
        UnPause();
    }

    private void Update()
    {
        // Handle pause input
        if (InputManager.playerInputs.PauseToggle) TogglePause(); 

        // If the game is paused, enable the UI and stop the game.
        if(isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; 
            menu.SetActive(true);
            Time.timeScale = 0; 
        }
        else // Do the opposite when its not paused
        {
            Time.timeScale = 1;
            menu.SetActive(false);
        }
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause() => isPaused = true;
    /// <summary>
    /// Unpauses the game
    /// </summary>
    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1;
        menu.SetActive(false);
        Cursor.visible = false; 
    }
    /// <summary>
    /// If the game is paused, un-pause it. If it is not paused, pause it.
    /// </summary>
    public void TogglePause() => isPaused = !isPaused;
    /// <summary>
    /// Load the main menu scene.
    /// </summary>
    public void MainMenu()
    {
        UnPause(); 
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuOrder);
    }
}
}