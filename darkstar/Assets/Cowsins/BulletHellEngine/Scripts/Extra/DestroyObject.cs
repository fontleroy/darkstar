using UnityEngine;

namespace cowsins.BulletHell
{

    public class DestroyObject : MonoBehaviour
    {
        [SerializeField] private float timeToDestroy;
        private void Start() => Destroy(gameObject, timeToDestroy);
    }
}