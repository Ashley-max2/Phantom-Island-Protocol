using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Controllers;

namespace UI {
    // popup para crear un login falso y generar una llave
    public class FakeLoginPopup : MonoBehaviour {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public Button confirmButton;
        public Button cancelButton;

        public TextMeshProUGUI titleText;
        public TextMeshProUGUI errorText;
        public TextMeshProUGUI infoText;

        private bool _isOpen = false;

        void Awake() {
            if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirm);
            if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);
            gameObject.SetActive(false);
        }

        public void Open() {
            _isOpen = true;
            gameObject.SetActive(true);

            if (usernameInput != null) usernameInput.text = "";
            if (passwordInput != null) passwordInput.text = "";
            if (errorText != null) errorText.text = "";
            if (titleText != null) titleText.text = "CREAR LOGIN FALSO";
            if (infoText != null) infoText.text = "Introduce credenciales para generar una llave de acceso.";

            Time.timeScale = 0f;
        }

        public void Close() {
            _isOpen = false;
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        public bool IsOpen() {
            return _isOpen;
        }

        private void OnConfirm() {
            string username = usernameInput != null ? usernameInput.text.Trim() : "";
            string password = passwordInput != null ? passwordInput.text.Trim() : "";

            if (string.IsNullOrEmpty(username)) {
                if (errorText != null) errorText.text = "ERROR: Username vacio.";
                return;
            }

            if (string.IsNullOrEmpty(password)) {
                if (errorText != null) errorText.text = "ERROR: Password vacio.";
                return;
            }

            if (username.Length < 3) {
                if (errorText != null) errorText.text = "ERROR: Username minimo 3 caracteres.";
                return;
            }

            // crear el user falso en la bd
            Database.UserRepository userRepo = new Database.UserRepository();
            bool created = userRepo.CreateFakeUser(username, password);

            if (!created) {
                if (errorText != null) errorText.text = "ERROR: Ese username ya existe.";
                return;
            }

            // gastar el laptop
            if (InventoryManager.Instance != null) {
                InventoryManager.Instance.RemoveItem(1, 1);
            }

            // generar la llave con el nombre del login
            if (HackingManager.Instance != null) {
                HackingManager.Instance.AddKey(username);
            }

            // meter la llave en el inventario
            if (InventoryManager.Instance != null) {
                bool added = InventoryManager.Instance.AddItem(4, 1);
                if (!added) {
                    Debug.LogWarning("No se pudo meter la llave (inventario lleno?).");
                }
            }

            Debug.Log($"Login falso creado! Llave \"{username}\" generada.");

            Close();
        }

        private void OnCancel() {
            Close();
        }
    }
}
