using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cowsins.BulletHell
{
    public class Damageable : MonoBehaviour, IDamageable
    {
        [System.Serializable]
        public class Events
        {
            public UnityEvent OnTakeDamage, OnTakeHeals, OnDie;
        }
        // Current health of the damageable object.
        [ReadOnly] public float health;

        // Current shield of the damageable object.
        [ReadOnly] public float shield;

        [Tooltip("Initial and maximum health of your player.")]
        public float maxHealth;

        [Tooltip("Initial and maximum shield of your player.")]
        public float maxShield;

        // Returns true if health < or = 0
        public bool isDead { get; private set; } = false;

        [SerializeField] private Events events;

        // Handle health settings initially.
        public virtual void Awake() => InitialSettings();

        // Death Detection
        public virtual void Update() => HandleDeath();

        /// <summary>
        /// Damages the damageable object. It affects both health and shield.
        /// </summary>
        /// <param name="damage">Damage to deal.</param>
        public virtual void TakeDamage(float damage)
        {
            // Invoke custom method if necessary
            events.OnTakeDamage?.Invoke();

            // If damage is greater than the shield remaining, also affect health.
            if (damage > shield)
            {
                // Calculate damage remaining and set shield to 0. Afterwards apply damage remaining on the health.
                float damageRemaining = damage - shield;
                shield = 0;
                health -= damageRemaining;
            }
            else shield -= damage; // Otherwise just change 
        }

        /// <summary>
        /// Heals the damageable object. It affects both health and shield.
        /// </summary>
        /// <param name="heal">Heal to apply.</param>
        public virtual void TakeHeals(float heal)
        {
            // Invoke custom method if necessary
            events.OnTakeHeals?.Invoke();

            // Check where to apply heal.
            if (heal + health > maxHealth && health < maxHealth)
            {
                float healsRemaining = maxHealth - health;
                health = maxHealth;
                if (heal > maxShield) shield = maxShield;
                else shield += healsRemaining;
            }
            else if (health >= maxHealth)
            {
                shield += heal;
                shield = Mathf.Clamp(shield, 0, maxShield);
            }
            else health += heal;
        }

        private void HandleDeath()
        {
            // Damageable is dead
            if (health <= 0) isDead = true;

            // Perform death behaviour.
            if (isDead) Die();
        }
        public virtual void Die()
        {
            // Invoke custom method if necessary
            events.OnDie?.Invoke();

            // Destroy the object.
            Destroy(this.gameObject);
        }

        // Handle health settings initially.
        private void InitialSettings()
        {
            health = maxHealth;
            shield = maxShield;
        }
    }
}