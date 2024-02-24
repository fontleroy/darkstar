using UnityEngine;

namespace cowsins.BulletHell
{
[RequireComponent(typeof(Animator))]
public class DestroyMe : MonoBehaviour
{
    private void Start() => Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length); 
}
}