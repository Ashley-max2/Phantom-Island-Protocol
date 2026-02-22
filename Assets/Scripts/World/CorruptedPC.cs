using UnityEngine;
using Controllers;

namespace World {
    [System.Serializable]
    public class FragmentRequirement {
        public int fragmentItemId = 2;
        public string displayName = "Fragmento";
        public int quantityRequired = 3;
    }

    // pc corrupto, necesitas llave + fragmentos para hackearlo
    public class CorruptedPC : MonoBehaviour {
        [Header("Popup")]
        public UI.CorruptedPCPopup pcPopup;

        [Header("Prompt [E]")]
        public GameObject interactPrompt;

        [Header("Config")]
        public string pcName = "PC_01";

        [Header("Fragmentos necesarios")]
        public FragmentRequirement[] requiredFragments = new FragmentRequirement[] {
            new FragmentRequirement { fragmentItemId = 2, displayName = "Fragmento", quantityRequired = 3 }
        };

        public bool isCompleted = false;

        [Header("Visual")]
        public SpriteRenderer spriteRenderer;
        public Color completedColor = Color.green;

        private bool _playerInRange = false;

        void Start() {
            if (interactPrompt != null) interactPrompt.SetActive(false);

            // si ya lo hackeaste antes, marcarlo
            if (HackingManager.Instance != null && HackingManager.Instance.IsPCAlreadyHacked(pcName)) {
                isCompleted = true;
                if (spriteRenderer != null) spriteRenderer.color = completedColor;
                Debug.Log($"PC {pcName} ya estaba completado.");
            }
        }

        void Update() {
            if (_playerInRange && Input.GetKeyDown(KeyCode.E) && !isCompleted) {
                if (pcPopup != null && pcPopup.IsOpen()) return;
                TryInteract();
            }
        }

        void TryInteract() {
            if (HackingManager.Instance == null) return;
            if (pcPopup == null) return;

            if (HackingManager.Instance.KeyCount() == 0) {
                Debug.Log($"ACCESO DENEGADO a {pcName}: no tienes llaves.");
                return;
            }

            pcPopup.Open(this);
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }

        // intenta completar el pc con una llave y los fragmentos
        public bool TryComplete(string keyName) {
            if (!HackingManager.Instance.HasKey(keyName)) {
                Debug.Log($"PC {pcName}: llave \"{keyName}\" no valida.");
                return false;
            }

            if (InventoryManager.Instance == null) return false;

            foreach (var req in requiredFragments) {
                if (!HasEnoughFragments(req.fragmentItemId, req.quantityRequired)) {
                    Debug.Log($"PC {pcName}: faltan fragmentos.");
                    return false;
                }
            }

            // consumir fragmentos
            foreach (var req in requiredFragments) {
                InventoryManager.Instance.RemoveItem(req.fragmentItemId, req.quantityRequired);
            }

            isCompleted = true;
            Debug.Log($"PC {pcName} COMPLETADO!");

            HackingManager.Instance.RegisterHackedPC(pcName);

            if (spriteRenderer != null) {
                spriteRenderer.color = completedColor;
            }

            if (interactPrompt != null) interactPrompt.SetActive(false);

            return true;
        }

        public bool HasEnoughFragments(int itemId, int quantityNeeded) {
            if (InventoryManager.Instance == null) return false;
            int totalOwned = 0;
            for (int i = 0; i < InventoryManager.MAX_SLOTS; i++) {
                var slot = InventoryManager.Instance.GetSlot(i);
                if (slot != null && slot.itemId == itemId) {
                    totalOwned += slot.quantity;
                }
            }
            return totalOwned >= quantityNeeded;
        }

        public int GetOwnedFragmentCount(int itemId) {
            if (InventoryManager.Instance == null) return 0;
            int total = 0;
            for (int i = 0; i < InventoryManager.MAX_SLOTS; i++) {
                var slot = InventoryManager.Instance.GetSlot(i);
                if (slot != null && slot.itemId == itemId) {
                    total += slot.quantity;
                }
            }
            return total;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                _playerInRange = true;
                if (!isCompleted && interactPrompt != null) {
                    interactPrompt.SetActive(true);
                }
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                _playerInRange = false;
                if (interactPrompt != null) interactPrompt.SetActive(false);
            }
        }
    }
}
