using UnityEngine;
using cowsins.BulletHell;
using TMPro;
using UnityEngine.SceneManagement; 

namespace cowsins.BulletHell
{
/// <summary>
/// This script works for the bullet spawner showroom. It showcases different bullet patterns, 
/// especially for the DEMO, so it won´t have much value for your game in most of the cases.
/// </summary>
public class ShowroomBulletSpawner : MonoBehaviour
{
    [SerializeField,Tooltip("Select the pattern spawner to work with.")] private PatternSpawner spawner;
    
    [SerializeField, Tooltip("Text that displays the current pattern and its name. Notice that this works using TMPro (TextMeshPro).")] private TextMeshProUGUI displayPatternText; 

    private int currentPattern = 0;

    // StartShooting
    private void Start() => spawner.Shoot(currentPattern, ShootingModes.Mode.Continuous);

    // Handle the UI
    private void Update() => displayPatternText.text = spawner.patterns[currentPattern].patternName + "\n" +(currentPattern + 1).ToString();

    /// <summary>
    /// Plays next pattern. 
    /// If the current pattern is the last one, it will go back to the first one.
    /// </summary>
    public void Next()
    {
        PoolManager.Instance.ClearPool(); 
        spawner.StopShooting();
        if (currentPattern == spawner.patterns.Length - 1) currentPattern = 0; 
        else currentPattern++;
        spawner.Shoot(currentPattern,ShootingModes.Mode.Continuous);
    }
    /// <summary>
    /// Plays the previous pattern.
    /// If the current pattern is the first one, it will travel to the last one.
    /// </summary>
    public void Previous()
    {
        PoolManager.Instance.ClearPool();
        spawner.StopShooting();
        if (currentPattern == 0) currentPattern = spawner.patterns.Length - 1;
        else currentPattern--;
        spawner.Shoot(currentPattern, ShootingModes.Mode.Continuous);
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