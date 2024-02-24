using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using cowsins.BulletHell; 
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{
public class MiniGameManager : MonoBehaviour
{
    [System.Serializable]
    public class Events 
    { 
        public UnityEvent OnPrepare, OnPlay, OnGameOver, OnVictory , OnMainMenu; 
    }

    [System.Serializable]
    public enum GameState
    {
        MainMenu,Preparing,Playing,GameOver, Victory, Other
    }

    public GameState gameState { get; private set; }

    [SerializeField,Tooltip("Assign your player object. When the player is not in the scene, that will count as GameOver. " +
        "(If the player is not in the scene or it is disabled, it means it died).")] private GameObject player; 

    [SerializeField, Tooltip("MainMenu Canvas. When leaving the main menu, this will be lerped." +
        " It MUST have a CanvasGroup assigned.")] private CanvasGroup mainMenu;

    [SerializeField, Tooltip("MainMenu fade out effect speed.")] private float mainMenuFadeSpeed; 

    [SerializeField, Tooltip("On Play, objects to enable. " +
        "This can be done through the events as well.")] private GameObject[] objectsToDisableOnPlay;

    [SerializeField, Tooltip("On Play, objects to disable. " +
        "This can be done through the events as well.")] private GameObject[] objectsToEnableOnPlay;

    [SerializeField, Tooltip("Array that contains all the possible enemies to instantiate / spawn.")] private GameObject[] enemies;

    [SerializeField, Tooltip("Maximum number of enemies that can be spawned at the same time. " +
        "If this value is set to 3, you will only be able to fight against three enemies at the same time.")] private int maxSimultaneousEnemies;

    [SerializeField, Tooltip("Maximum number of enemies to spawn. Once all the enemies have been killed, " +
        "the game state will change to Victory.")] private int enemiesToSpawn;

    [SerializeField, Tooltip("Once an enemy is dead, time remaining to spawn a new one." +
        " Set this to 0 for instant spawn.")] private float spawnInterval;

    [SerializeField, Tooltip("Transforms / Locations where these enemies will randomly be spawned.")] private Transform[] spawnPoints;

    [SerializeField, Tooltip("Text that displays the enemies left. " +
        "Notice that this works using TMPro (TextMeshPro).")] private TMPro.TextMeshProUGUI enemiesLeftDisplay; 

    [SerializeField] private Events events;

    private int enemiesLeft; 


    // Handle initial settings
    private void Start()
    {
        // Set initial game state
        gameState = GameState.MainMenu;

        // Play custom methods
        events.OnMainMenu?.Invoke();

        // Handle initial enemy logic
        enemiesLeft = enemiesToSpawn;
        if (enemiesLeftDisplay != null) enemiesLeftDisplay.text = enemiesLeft.ToString();

        Cursor.visible = true;
    }

    private void Update()
    {
        HandleMainMenu();
        HandleGameOver();
    }

    public void Prepare()
    {
        // If we are not on the main menu, we can´t proceed to Prepare the 
        if (gameState != GameState.MainMenu) return;

        Cursor.visible = false;

        // Play custom method
        events.OnPrepare?.Invoke(); 

        // Set game state
        gameState = GameState.Preparing;

        for (int i = 0; i < objectsToDisableOnPlay.Length; i++) objectsToDisableOnPlay[i].SetActive(false);

        for (int i = 0; i < objectsToEnableOnPlay.Length; i++) objectsToEnableOnPlay[i].SetActive(true);

        // Start the countdown
        StartCoroutine(Countdown()); 
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(3.1f);
        Play(); 
    }
    /// <summary>
    /// Changes the game state to Playing and runs Play logic on game state changed.
    /// </summary>
    public void Play()
    {
        events.OnPlay?.Invoke();
        gameState = GameState.Playing;

        for (int i = 0; i < maxSimultaneousEnemies; i++)
            SpawnEnemy(); 
    }
    /// <summary>
    /// Changes the game state to GameOver and runs GameOver logic on game state changed.
    /// </summary>
    public void GameOver()
    {
        events.OnGameOver?.Invoke();
        gameState = GameState.GameOver;
    }
    private void HandleMainMenu()
    {
        // If the main menu has already been completely faded out, and it exists, disable it
        if (mainMenu != null && mainMenu.alpha <= 0)
        {
            mainMenu.gameObject.SetActive(false);
            return; 
        }
        
        // If we are on the main menu, do not fade out the canvas UI.
        if (gameState == GameState.MainMenu) return;
        mainMenu.alpha -= Time.deltaTime * mainMenuFadeSpeed; 
    }

    // Called when the enemy dies. Checks for remaining enemies and displays the value on the UI
    private void RemoveEnemy()
    {
        enemiesLeft--;
        if(enemiesLeftDisplay != null) enemiesLeftDisplay.text = enemiesLeft.ToString();
        if (enemiesLeft <= 0)
        {     
            CancelInvoke(nameof(SpawnEnemy));  
            HandleVictory(); 
            return; 
        }
        Invoke(nameof(SpawnEnemy), spawnInterval); 
    }

    private void SpawnEnemy()
    {
        // Instantiate the enemy
        var en = Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        // Assign the onDeath event to the RemoveEnemy method in this script
        if(en.GetComponent<Enemy>() != null) en.GetComponent<Enemy>().onDeath = RemoveEnemy;
    }
    /// <summary>
    /// Changes the game state to Victory.
    /// </summary>
    public void HandleVictory()
    {
        // Play custom method
        events.OnVictory?.Invoke();

        // Change the state
        gameState = GameState.Victory; 
    }
    private void HandleGameOver()
    {
        if (enemiesLeft <= 0) gameState = GameState.Victory;

        if (gameState == GameState.Playing && player == null) GameOver(); 
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


#if UNITY_EDITOR

[CustomEditor(typeof(MiniGameManager))]
public class MiniGameManagerEditor : Editor
{
    private string[] tabs = { "Main", "Spawn", "Events" };
    private int currentTab = 0;

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        var myScript = target as MiniGameManager;

        EditorGUILayout.BeginVertical();
        currentTab = GUILayout.Toolbar(currentTab, tabs);
        EditorGUILayout.Space(5f);
        EditorGUILayout.EndVertical();

        if (currentTab >= 0 || currentTab < tabs.Length)
        {
            switch (tabs[currentTab])
            {
                case "Main":
                    EditorGUILayout.LabelField("MAIN", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mainMenu")); 
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("objectsToEnableOnPlay"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("objectsToDisableOnPlay")); 
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mainMenuFadeSpeed"));
                    break;
                case "Spawn":
                    EditorGUILayout.LabelField("SPAWNING ENEMIES", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("enemies"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("enemiesToSpawn")); 
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnInterval"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSimultaneousEnemies"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnPoints"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("enemiesLeftDisplay"));
                    break;
                case "Events":
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("events"));
                    break;
            }
        }

        serializedObject.ApplyModifiedProperties();

    }
}
#endif
}