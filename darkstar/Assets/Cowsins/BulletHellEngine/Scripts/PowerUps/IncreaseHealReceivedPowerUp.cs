using UnityEngine;

namespace cowsins.BulletHell
{ 
    public class IncreaseHealReceivedPowerUp : MonoBehaviour
    {
        [SerializeField, Range(.01f,1),Tooltip("value added to the heal received modifier in PlayerModifiers.cs")] private float healReceived;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            collision.GetComponent<PlayerModifiers>().healReceivedModifier += healReceived;
            Destroy(this.gameObject);
        }
    }
}