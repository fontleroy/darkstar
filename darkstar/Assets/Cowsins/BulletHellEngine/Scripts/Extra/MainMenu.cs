using UnityEngine;
using UnityEngine.SceneManagement; 

namespace cowsins.BulletHell
{
    public class MainMenu : MonoBehaviour
    {
        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; 
        }
        public void LoadScene(int id) => SceneManager.LoadSceneAsync(id);

        public void Quit() => Application.Quit(); 

    }
}