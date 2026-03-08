using UnityEngine;
using TMPro; 
using Database;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Models;

namespace Controllers {
    public class AuthenticationManager : MonoBehaviour {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public TextMeshProUGUI errorText;
        public TextMeshProUGUI userListDisplay; // opcional, para mostrar la lista

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

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
                 if (errorText != null) errorText.text = "Fill all fields.";
                 return;
            }

            if(_userRepo.CreateFakeUser(username, password)) {
                 Debug.Log("User created!");
                 if (errorText != null) errorText.text = "User created successfully!";
                 OnListUsersButton(); // refresca la lista
            } else {
                 Debug.Log("User already exists.");
                 if (errorText != null) errorText.text = "User already exists.";
            }
        }

        // muestra todos los usuarios en el console o en un texto
        public void OnListUsersButton() {
            List<User> users = _userRepo.GetUsers();
            string listText = "Registered Users:\n";
            
            foreach (var u in users) {
                listText += $"- {u.username} (ID: {u.id}, Fake: {u.isFake})\n";
            }

            Debug.Log(listText);
            if (userListDisplay != null) {
                userListDisplay.text = listText;
            }
        }

        // borra el usuario que esté escrito en el input de username
        public void OnDeleteUserButton() {
            string username = usernameInput.text;
            var users = _userRepo.GetUsers();
            var target = users.Find(u => u.username == username);

            if (target != null) {
                _userRepo.DeleteUser(target.id);
                Debug.Log($"User {username} deleted.");
                if (errorText != null) errorText.text = $"User {username} deleted.";
                OnListUsersButton(); // refresca
            } else {
                Debug.Log("User not found to delete.");
            }
        }

        // borra absolutamente todos los usuarios de la base de datos
        public void OnDeleteAllUsersButton() {
            _userRepo.DeleteAllUsers();
            Debug.Log("Database wiped.");
            if (errorText != null) errorText.text = "All users deleted.";
            OnListUsersButton(); // refresca para que salga vacio
        }
    }
}



