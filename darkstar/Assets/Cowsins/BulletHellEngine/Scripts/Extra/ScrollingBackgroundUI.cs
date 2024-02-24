using UnityEngine;
using UnityEngine.UI; 

namespace cowsins.BulletHell
{ 
    public class ScrollingBackgroundUI : MonoBehaviour
    {
        private RawImage image;

        [SerializeField]private Vector2 speed;

        private void Start() => image = GetComponent<RawImage>(); 

        private void Update() => image.uvRect = new Rect(image.uvRect.position + speed * Time.deltaTime, image.uvRect.size); 
    }
}