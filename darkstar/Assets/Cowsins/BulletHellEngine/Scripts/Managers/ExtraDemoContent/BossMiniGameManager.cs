using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 

namespace cowsins.BulletHell
{
/// <summary>
/// Specially designed for the DEMO, 
/// but you can use it for your own game too! Handles basic behaviour for a simple mini game loop for a boss scene.
/// Aimed at arcade style, mainly.
/// </summary>
public class BossMiniGameManager : MonoBehaviour
{
    [System.Serializable]
    public class Events
    {
        public UnityEvent OnPrepare, OnPlay, OnGameOver, OnVictory, OnMainMenu;
    }
    [System.Serializable]
    public enum GameState
    {
        MainMenu,Preparing,Playing,Victory,GameOver
    }

    //Returns the current GameState.
    //Example of return value:
    //GameState.Playing
    public GameState gameState { get; private set; }

    [SerializeField, Tooltip("Assign your player object. When the player is not in the scene, that will count as GameOver. " +
        "(If the player is not in the scene or it is disabled, it means it died).")]
    private GameObject player;

    [SerializeField,Tooltip("MainMenu Canvas. When leaving the main menu, this will be lerped. " +
        "It MUST have a CanvasGroup assigned.")] private CanvasGroup mainMenu;

    [SerializeField, Tooltip("MainMenu fade out effect speed.")] private float mainMenuFadeSpeed;

    [SerializeField, Tooltip("Boss object.")] private Transform boss; 

    [SerializeField, Tooltip("What the initial position of the boss should be. " +
        "You can place it anywhere else, and the boss will move towards the position you specify here. " +
        "This is a very simple animation through code.")] private Vector3 initialBossPosition;

    [SerializeField, Tooltip("For the animation stated before, lerp speed from the current position to the initial position of the boss.")] private float bossSpeed; 

    [SerializeField] private Events events;

    private void Start()
    {
        // Call our custom method on start
        events.OnMainMenu?.Invoke();

        Cursor.visible = true; 
    }
    private void Update()
    {
        HandleMainMenu();
        HandlePreparation();
        HandleVictory();
        HandleGameOver(); 
    }
    /// <summary>
    /// Changes the game state to Preparing and runs Prepare logic on game state changed.
    /// </summary>
    public void Prepare()
    {
        // We do not want to prepare the game if we are not in the main menu
        if (gameState != GameState.MainMenu) return;

        Cursor.visible = false; 
        //Call custom method
        events.OnPrepare?.Invoke();

        // Change the game state
        gameState = GameState.Preparing;

        // Activate our boss
        boss.gameObject.SetActive(true);
        if(boss.GetComponent<BossExample>() != null)
            boss.GetComponent<BossExample>().enabled = false;
    }

    private void HandlePreparation()
    {
        // Handles logic related to preparation only if the game state is equal to preparing
        if (gameState != GameState.Preparing) return;

        //Boss animation
        boss.position = Vector3.MoveTowards(boss.position, initialBossPosition, bossSpeed * Time.deltaTime);

        // if the boss matches the start position, we can start playing
        if (boss.position == initialBossPosition) Play(); 
    }
    /// <summary>
    /// Changes the game state to Playing and runs Play logic on game state changed.
    /// </summary>
    public void Play()
    {
        //Call custom method
        events.OnPlay?.Invoke();

        // Change the game state
        gameState = GameState.Playing;

        // Enable damaging the boss
        if (boss.GetComponent<BossExample>() != null)
            Invoke(nameof(EnableBoss),.5f); 
    }

    private void EnableBoss() => boss.GetComponent<BossExample>().enabled = true; 

    // if the boss is dead, you won!
    private void HandleVictory()
    {
        if (boss == null && gameState == GameState.Playing) Victory(); 
    }
    /// <summary>
    /// Changes the game state to Victory and runs Victory logic on game state changed.
    /// </summary>
    public void Victory()
    {
        // Custom method
        events.OnVictory?.Invoke();

        // Change the game state
        gameState = GameState.Victory; 
    }
    /// <summary>
    /// Changes the game state to GameOver and runs GameOver logic on game state changed.
    /// </summary>
    public void GameOver()
    {
        // Custom method
        events.OnGameOver?.Invoke();
        // Change the game state
        gameState = GameState.GameOver; 
    }

    private void HandleGameOver()
    {
        if (gameState == GameState.Playing && player == null) GameOver();
    }

    // Handle the main menu canvas effect
    private void HandleMainMenu()
    {
        // If the main menu is completely done fading out, disable it
        if (mainMenu != null && mainMenu.alpha <= 0)
        {
            mainMenu.gameObject.SetActive(false);
            return;
        }

        // Only fade the main menu out if we are not on the main menu state.
        if (gameState == GameState.MainMenu) return;
        mainMenu.alpha -= Time.deltaTime * mainMenuFadeSpeed;
    }
    /// <summary>
     /// Restarts the current scene.
     /// </summary>
    public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    /// <summary>
    /// Loads certain scene given its order ID (check this in File/BuildSettings)
    /// </summary>
    /// <param name="i">Scene to load by its order ID. (check this in File/BuildSettings)</param>
    public void LoadScene(int i) => SceneManager.LoadScene(i);
}
}