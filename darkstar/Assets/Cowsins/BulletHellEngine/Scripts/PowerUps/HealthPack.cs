using UnityEngine;

namespace cowsins.BulletHell
{ 
    public class HealthPack : MonoBehaviour
    {
        [SerializeField, Tooltip("Heal amount to take (for Player).")] private float health;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return; 

            collision.GetComponent<PlayerHealth>().TakeHeals(health);
            Destroy(this.gameObject);
        }
    }
}