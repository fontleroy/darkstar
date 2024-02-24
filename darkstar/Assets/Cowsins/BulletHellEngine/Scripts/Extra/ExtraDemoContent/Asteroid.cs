using UnityEngine;
using cowsins.BulletHell; 

namespace cowsins.BulletHell
{
/// <summary>
/// Handles the asteroid behaviour shown in the demos. Notice that there is an asteroid prefab that you can use if needed.
//& This damages the player on trigger.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Asteroid : MonoBehaviour
{
    [SerializeField,Tooltip("Damage dealt to the player on trigger.")] private float damage;

    [SerializeField, Tooltip("Array that contains sprites. At start, the sprite will be randomly selected among these.")] private Sprite[] randomSprites; 

    // Grab the sprite renderer reference.
    private void Start() => GetComponent<SpriteRenderer>().sprite = randomSprites[Random.Range(0, randomSprites.Length)]; 

    // Handle trigger behaviour.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collision object was the player.
        if (collision.CompareTag("Player"))
        {
            // Apply damage to the player.
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);

            // Destroy the object afterwards.
            Destroy(this.gameObject); 
        }

    }
}
}