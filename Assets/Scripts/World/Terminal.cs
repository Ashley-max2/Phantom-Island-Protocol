using UnityEngine;
using Controllers;

namespace World {
    // terminal para crear logins falsos y generar llaves
    public class Terminal : MonoBehaviour {
        public UI.FakeLoginPopup loginPopup;
        public GameObject interactPrompt;

        private bool playerInRange = false;

        void Start() {
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }

        void Update() {
            if (playerInRange && Input.GetKeyDown(KeyCode.E)) {
                if (loginPopup != null && loginPopup.IsOpen()) return;
                TryHack();
            }
        }

        void TryHack() {
            if (InventoryManager.Instance == null) return;
            if (loginPopup == null) return;

            if (InventoryManager.Instance.HasItem(1)) {
                loginPopup.Open();
                if (interactPrompt != null) interactPrompt.SetActive(false);
                Debug.Log("Terminal: creando login falso...");
            } else {
                Debug.Log("Necesitas un Laptop para usar el terminal.");
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
             if (other.CompareTag("Player")) {
                 playerInRange = true;
                 if (interactPrompt != null) interactPrompt.SetActive(true);
             }
        }
        
        void OnTriggerExit2D(Collider2D other) {
             if (other.CompareTag("Player")) {
                 playerInRange = false;
                 if (interactPrompt != null) interactPrompt.SetActive(false);
             }
        }
    }
}
