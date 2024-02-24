using UnityEngine;

namespace cowsins.BulletHell
{
public class ExtraSpawnables : MonoBehaviour
{
    private MiniGameManager mng;

    [SerializeField,Tooltip("Array that contains the game objects to spawn.")] private GameObject[] extras;

    [SerializeField, Tooltip("Spawn points (Transforms) where the objects will be spawned.")] private Transform[] spawns; 

    [SerializeField, Tooltip("Instantiate pace. The lower this value, the faster these objects will be spawned.")] private float spawnInterval; 

    private bool hasInstantiated; 

    private void Awake()
    {
        // Grab the manager reference
        mng = GetComponent<MiniGameManager>(); 
    }

    private void Update()
    {
        // Do not spawn if we are not playing or we have already done that
        if (mng.gameState != MiniGameManager.GameState.Playing || hasInstantiated) return;

        // Instantiate a random extra in a random spawn
        Instantiate(extras[Random.Range(0, extras.Length)], spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity); 

        // Check that we have instantiated
        hasInstantiated = true;

        // Restart the logic
        Invoke(nameof(RepeatShot), spawnInterval); 
    }

    private void RepeatShot() => hasInstantiated = false; 
}
}