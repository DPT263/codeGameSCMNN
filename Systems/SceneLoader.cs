using UnityEngine;
using UnityEngine.SceneManagement;

namespace SocMuaNuocNoi
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void LoadNextScene()
        {
            LoadScene(nextSceneName);
        }

        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadMenu()
        {
            LoadScene("Menu");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
