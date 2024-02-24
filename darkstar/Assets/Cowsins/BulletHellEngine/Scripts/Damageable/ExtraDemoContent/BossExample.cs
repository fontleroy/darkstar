using UnityEngine;

namespace cowsins.BulletHell
{
public class BossExample : MonoBehaviour
{
    [SerializeField,Tooltip("Attach the spawners from the sides")] private PatternSpawner[] muzzleSpawners;

    [SerializeField,Tooltip("Attach the central pattern spawner")] private PatternSpawner generalSpawner; 

    private int currentStage = 1;

    private Enemy enemy; 

    private void Start()
    {
        // Initially Shoot on each muzzle continuosly
        foreach(var muz in muzzleSpawners)
        {
            muz.Shoot(currentStage - 1, cowsins.BulletHell.ShootingModes.Mode.Continuous); 
        }

        // Grab the enemy reference
        enemy = GetComponent<Enemy>(); 
    }

    private void Update()
    {
        HandleStages(); 
    }

    private void HandleStages()
    {
        // Handle different stage behaviours
        // If the enemy is more than 75% health, stage = 1
        if (enemy.health > enemy.maxHealth * .75f) currentStage = 1;

        // if the enemy is less than 75%, stage = 2
        if (enemy.health < enemy.maxHealth * .75f && currentStage == 1)
        {
            currentStage = 2;
            // Stop shooting form muzzles
            foreach (var muz in muzzleSpawners)
            {
                muz.StopShooting();
            }
            // Shoot from the center continuously.
            generalSpawner.Shoot(0, cowsins.BulletHell.ShootingModes.Mode.Continuous);
        }
        // if the enemy is less than 50%, stage =3
        if (enemy.health < enemy.maxHealth * .5f && currentStage == 2)
        {
            currentStage = 3;
            // Stop shooting from the center
            generalSpawner.StopShooting();

            // Shoot a different pattern from the center.
            generalSpawner.Shoot(1, cowsins.BulletHell.ShootingModes.Mode.Continuous);

            // Start Shooting for each muzzle spawner
            foreach (var muz in muzzleSpawners)
            {
                muz.StopShooting();
                muz.Shoot(1, cowsins.BulletHell.ShootingModes.Mode.Continuous);
            }
        }
        // if the enemy is less than 25%, stage = 4
        if (enemy.health < enemy.maxHealth * .25f && currentStage == 3)
        {
            currentStage = 4;
            // Stop shooting
            foreach (var muz in muzzleSpawners)
            {
                muz.StopShooting();
            }
            
            // Perform central image pattern shooting
            generalSpawner.Shoot(2, cowsins.BulletHell.ShootingModes.Mode.Continuous);
        }
    }
}
}