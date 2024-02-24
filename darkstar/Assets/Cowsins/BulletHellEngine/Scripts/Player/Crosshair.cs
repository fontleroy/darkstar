using UnityEngine;
using cowsins.BulletHell;

namespace cowsins.BulletHell
{ 
    /// <summary>
    /// Handles the crosshair object behaviour. It automatically gets destroyed for Android and iOS builds.
    /// </summary>
    public class Crosshair : MonoBehaviour
    {
        #if UNITY_IOS || UNITY_ANDROID
        private void Start()
        {
            Destroy(this.gameObject); 
        }
        #endif

        private void Update()
        {
            var mousePos = (Vector3)InputManager.playerInputs.MousePos;
            mousePos.z = 10.0f;
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }
}