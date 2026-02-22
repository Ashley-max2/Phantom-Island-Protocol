using UnityEngine;
using TMPro; 
using Database;
using UnityEngine.SceneManagement;

namespace Controllers {
    public class AuthenticationManager : MonoBehaviour {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public TextMeshProUGUI errorText;

        private UserRepository _userRepo;

        void Start() {
            _userRepo = new UserRepository();
            if (errorText != null) errorText.text = "";
        }

        // cuando le das al boton de login
        public void OnLoginButton() {
            string username = usernameInput.text;
            string password = passwordInput.text;

            var user = _userRepo.Login(username, password);

            if (user != null) {
                GameManager.Instance.SetUser(user);
                GameManager.Instance.StartGame();
                try {
                    SceneManager.LoadScene("IslandMain"); 
                } catch {
                    Debug.LogWarning("Scene 'IslandMain' not found.");
                }
            } else {
                if (errorText != null) errorText.text = "Invalid credentials.";
                Debug.Log("Login failed.");
            }
        }

        // registra un user falso (para generar llaves)
        public void OnRegisterFakeButton() {
            string username = usernameInput.text;
            string password = passwordInput.text;
             if(_userRepo.CreateFakeUser(username, password)) {
                 Debug.Log("User created!");
             } else {
                 Debug.Log("User already exists.");
             }
        }
    }
}
