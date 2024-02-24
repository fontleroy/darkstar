using UnityEngine;

namespace cowsins.BulletHell
{
    public class DecreaseDamageReceivedPowerUp : MonoBehaviour
    {
        [SerializeField,Range(-1f,-.01f), Tooltip("")] private float damagedReceived;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            collision.GetComponent<PlayerModifiers>().damageReceivedModifier += damagedReceived;
            Destroy(this.gameObject);
        }
    }
}