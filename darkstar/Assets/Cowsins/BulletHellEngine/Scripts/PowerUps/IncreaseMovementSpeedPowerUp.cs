using UnityEngine;

namespace cowsins.BulletHell
{
    public class IncreaseMovementSpeedPowerUp : MonoBehaviour
    {
        [SerializeField, Range(.01f, 1), Tooltip("movement speed value added to the movement speed modifier in PlayerModifiers.cs")] private float movementSpeed;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;
            print(collision.GetComponent<PlayerModifiers>().movementSpeedModifier);
            collision.GetComponent<PlayerModifiers>().movementSpeedModifier += movementSpeed;
            Destroy(this.gameObject);
        }
    }

}