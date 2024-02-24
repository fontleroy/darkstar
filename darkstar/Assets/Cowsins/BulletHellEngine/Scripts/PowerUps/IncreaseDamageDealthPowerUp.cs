using UnityEngine;

namespace cowsins.BulletHell
{ 
    public class IncreaseDamageDealthPowerUp : MonoBehaviour
    {
        [SerializeField, Range(.01f, 1), Tooltip("value added to the damage dealt modifier in PlayerModifiers.cs")] private float damageDealt;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            collision.GetComponent<PlayerModifiers>().damageDealtModifier += damageDealt;
            Destroy(this.gameObject);
        }
    }
}
