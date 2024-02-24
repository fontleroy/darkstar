using UnityEngine;
using TMPro;

namespace cowsins.BulletHell
{
/// <summary>
/// Handles scoring. The object that has this component attached is generally included in the player controller prefab.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // Returns the current score.
    public int score { get; private set; }

    [SerializeField,Tooltip("If true, the score can be negative. If false, the minimum score will be clamped to 0.")] private bool canScoreNegative;

    [SerializeField, Tooltip("UI text that displays the current score. This uses TMPro (TextMeshPro).")] private TextMeshProUGUI scoreDisplay;

    [SerializeField,Min(0), Tooltip("the scoreDisplay performs a code-driven animation when the score value changes, " +
        "this is the intensity of the animation.")] private float scoreEffectAmount = 1.1f;

    [SerializeField, Tooltip("the scoreDisplay performs a code-driven animation when the score value changes, " +
        "this is the lerp speed to its original state.")] private float scoreEffectSpeed = 5;

    public static ScoreManager Instance { get; private set; }
    private void Awake()
    {
        // Handle singleton
        if (Instance != null && Instance != this) Destroy(this);
        else  Instance = this;
    }

    private void Update() => HandleScoreUI(); 

    /// <summary>
    /// Adds a certain amount of points to the player score.
    /// </summary>
    /// <param name="amount">Amount to add.</param>
    public void AddScore(int amount)
    {
        score += amount;
        // Score effect
        if (scoreDisplay != null) scoreDisplay.transform.localScale = Vector3.one * scoreEffectAmount; 
    }

    /// <summary>
    /// Remove a certain amount of points to the player score. Score will be reset to 0 in case it is negative aand that´s not allowed.
    /// You can allow negative values for the score on the ScoreManager. Check "canScoreNegative".
    /// </summary>
    /// <param name="amount">Amount to remove.</param>
    public void RemoveScore(int amount)
    {
        score -= amount;

        // Check if the score should be reset.
        if (score < 0 && !canScoreNegative) score = 0;
        // Score effect
        if (scoreDisplay != null) scoreDisplay.transform.localScale = Vector3.one * scoreEffectAmount;
    }

    private void HandleScoreUI()
    {
        // Check if we should proceed or not.
        if (scoreDisplay == null)
        {
            Debug.LogWarning(" Score Display is missing form Score Manager. Please assign a TextMeshProUGUI, otherwise Score won´t be displayed.");
            return; 
        }

        // Assign score value to our display text.
        scoreDisplay.text = score.ToString();

        // Reset scale
        if (scoreDisplay.transform.localScale != Vector3.one) scoreDisplay.transform.localScale = Vector3.Lerp(scoreDisplay.transform.localScale, Vector3.one, Time.deltaTime * scoreEffectSpeed); 
    }
}
}