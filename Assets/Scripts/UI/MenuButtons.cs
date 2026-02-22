using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class MenuButtons : MonoBehaviour {

        // vuelve a la pantalla de login
        public void GoToLogin() {
            if (Controllers.GameManager.Instance != null) {
                Controllers.GameManager.Instance.isGameActive = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(0);
        }

        // cierra el juego
        public void QuitGame() {
            Debug.Log("Cerrando el juego...");

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
