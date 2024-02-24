using UnityEngine;

namespace cowsins.BulletHell
{
    public class PlayerModifiers : MonoBehaviour
    {
        //Multiplies the player movement speed.
        [HideInInspector]public float movementSpeedModifier = 1;

        //Multiplies the damage dealt by the player.
        [HideInInspector] public float damageDealtModifier = 1;

        //Multiplies the damage received by the player.
        [HideInInspector] public float damageReceivedModifier = 1;

        //Multiplies the heal received by the player.
        [HideInInspector] public float healReceivedModifier = 1;

        [HideInInspector] public static PlayerModifiers Instance;

        // Handle Singleton
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject); 
        }
    }
}