using UnityEngine;
using System.Collections; 

namespace cowsins.BulletHell
{
public class SimpleEnemyExample : MonoBehaviour
{
    [SerializeField,Tooltip("Attach a pattern spawner. The enemy will shoot from there.")] private PatternSpawner patternSpawner;

    [SerializeField, Tooltip("Pivot used to rotate the weapon sprite. This will aim towards the player.")] private Transform weaponPivot;

    [SerializeField, Tooltip("Movement Speed of the enemy. Note that this enemy example performs random movements.")] private float moveSpeed;

    [SerializeField, Tooltip("Minimum time to invert directions.")] private float minTimeToChangeDir;

    [SerializeField, Tooltip("Maximum time to invert directions. ")] private float maxTimeToChangeDir;

    private Transform player;

    [SerializeField, Tooltip("Starting movement direction of the enemy.")] private Vector2 movementDirection = new Vector2(.2f,1); 


    private void Start()
    {
        // Perform initial settings
        InitialEnemySettings();

        // Start Shooting continuosly 
        patternSpawner.Shoot(0, cowsins.BulletHell.ShootingModes.Mode.Continuous);

        // Handle movement
        StartCoroutine(HandleMovement()); 
    }

    private void Update()
    {
        // Return if the player is no longer in the scene
        if (player == null) return; 

        // If there is a weapon pivot attached, aim it towards the player.
        if(weaponPivot != null)
        weaponPivot.right = transform.position - player.position;
    }
    private void FixedUpdate()
    {
        // If the player is not dead, move it in the current movement direction.
        if (GetComponent<Damageable>().isDead) return;

        GetComponent<Rigidbody2D>().AddForce(movementDirection * moveSpeed * Time.deltaTime, ForceMode2D.Force);
    }

    private void InitialEnemySettings()
    {
        // Get the player reference by its tag "Player".
        if (GameObject.FindGameObjectWithTag("Player") == null) return; 
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private IEnumerator HandleMovement()
    {
        // Wait a random amount of time ( between two values that you can set in the inspector )
        yield return new WaitForSeconds(Random.Range(minTimeToChangeDir, maxTimeToChangeDir));

        // Reverse / Invert the direction
        movementDirection = -movementDirection; 

        // Restart
        StartCoroutine(HandleMovement());
    }
}
}