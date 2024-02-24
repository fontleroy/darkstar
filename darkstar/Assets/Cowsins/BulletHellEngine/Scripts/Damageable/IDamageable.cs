using UnityEngine;

namespace cowsins.BulletHell
{
    // Interface that handles damage systems of an object. Damageable.cs inherits from IDamageable.cs.
    public interface IDamageable
    {
        /// <summary>
        /// Damages the damageable object. It affects both health and shield.
        /// </summary>
        /// <param name="damage">Damage to deal.</param>
        public void TakeDamage(float damage);

        public void Die(); 
    }
}