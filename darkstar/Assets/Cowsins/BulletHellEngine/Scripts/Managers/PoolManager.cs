using UnityEngine;
using System.Collections.Generic;

namespace cowsins.BulletHell
{
    /// <summary>
    /// The Pool Manager is a system that helps to improve the performance of a game significantly. 
    /// The pool managers stores objects in an array, and instead of generating garbage by destroying 
    /// and instantiating new instances of the same object, it enables and disables them,
    /// returning them back to the pool.
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        [Tooltip("Attach the generic prefab for the bullets: Cowsins_Bullet.prefab")]public Bullet bullet;

        [SerializeField,Tooltip("DO NOT ATTACH ANYTHING. This is shown to control the bullets in the pool currently.")] private List<Bullet> bulletPool;

        [SerializeField, Tooltip("Number of bullets to store in the pool. Notice that all of these bullets will be instantiated at once on start ( Pool Initialization ). " +
            "Notice as well that it is STRONGLY RECOMMENDED to tweak this value until you are satisfied. " +
            "It makes no sense to set such a high value that a large amount of the bullets stored in the pool are never used. " +
            "As well as it makes no sense to set such a low value that the system needs to spawn new instances each time.")] private int bullets;

        private Transform poolContainer;

        private void Awake()
        {
            // Handle singleton behavuour
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;

            // The pool container will be this transform.
            poolContainer = transform; 
        }

        // Initialize the pool at start.
        // It is VERY IMPORTANT to read the recommendations of the variable bullets ( Check the tooltip ).
        // Notice that this is also stated in the documentation
        private void Start() => bulletPool = InitializePool(bullets); 

        // Spawn the require bullets for the pool and store them
        private List<Bullet> InitializePool(int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                Bullet bul = Instantiate(bullet);
                bul.transform.parent = poolContainer;
                bulletPool.Add(bul);
                bul.gameObject.SetActive(false);
                bul.enabled = false;
            }

            return bulletPool;
        }
        /// <summary>
        /// Returns a bullet from the pool
        /// </summary>
        /// <returns>Bullet.cs</returns>
        public Bullet RequestBullet()
        {
            foreach(var bul in bulletPool)
            {
                if(bul.gameObject.activeSelf == false)
                {
                    bul.gameObject.SetActive(true);
                    bul.enabled = true;
                    return bul; 
                }
            }
            Bullet newBul = Instantiate(bullet,poolContainer);
            bulletPool.Add(newBul);
            return newBul; 
        }
        /// <summary>
        /// Sends back all the bullets to the pool
        /// </summary>
        public void ClearPool()
        {
            foreach(var bul in bulletPool)
            {
                if (bul.gameObject.activeSelf != false) bul.DestroyMe();    
            }
        }
    }

}