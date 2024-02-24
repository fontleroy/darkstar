using UnityEngine;

namespace cowsins.BulletHell
{ 
    public class LookAtCamera : MonoBehaviour
    {
        private void Update() => transform.LookAt(Camera.main.transform);  
    }
}
