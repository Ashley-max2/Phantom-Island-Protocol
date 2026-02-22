using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Controllers;

namespace UI {
    // popup para hackear un pc corrupto
    public class CorruptedPCPopup : MonoBehaviour {
        public TMP_Dropdown keyDropdown;
        public Button confirmButton;
        public Button cancelButton;

        public TextMeshProUGUI titleText;
        public TextMeshProUGUI errorText;
        public TextMeshProUGUI fragmentsInfoText;

        private bool _isOpen = false;
        private World.CorruptedPC _currentPC;

        void Awake() {
            if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirm);
            if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);
            gameObject.SetActive(false);
        }

        public void Open(World.CorruptedPC pc) {
            if (pc == null) return;

            _currentPC = pc;
            _isOpen = true;
            gameObject.SetActive(true);

            if (errorText != null) errorText.text = "";
            if (titleText != null) titleText.text = $"PC CORRUPTO: {pc.pcName}";

            PopulateKeyDropdown();
            UpdateFragmentsInfo();

            Time.timeScale = 0f;
        }

        public void Close() {
            _isOpen = false;
            _currentPC = null;
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        public bool IsOpen() {
            return _isOpen;
        }

        // rellena el dropdown con las llaves disponibles
        private void PopulateKeyDropdown() {
            if (keyDropdown == null || HackingManager.Instance == null) return;

            keyDropdown.ClearOptions();
            var keys = HackingManager.Instance.GetKeys();

            if (keys.Count == 0) {
                keyDropdown.AddOptions(new System.Collections.Generic.List<string> { "-- Sin llaves --" });
                if (confirmButton != null) confirmButton.interactable = false;
            } else {
                var options = new System.Collections.Generic.List<string>();
                foreach (var key in keys) {
                    options.Add($"Llave \"{key}\"");
                }
                keyDropdown.AddOptions(options);
                if (confirmButton != null) confirmButton.interactable = true;
            }
        }

        // muestra cuantos fragmentos tienes vs los que necesitas
        private void UpdateFragmentsInfo() {
            if (fragmentsInfoText == null || _currentPC == null) return;

            if (_currentPC.requiredFragments == null || _currentPC.requiredFragments.Length == 0) {
                fragmentsInfoText.text = "No requiere fragmentos.";
                return;
            }

            string info = "<b>FRAGMENTOS NECESARIOS:</b>\n";
            bool allMet = true;

            foreach (var req in _currentPC.requiredFragments) {
                int owned = _currentPC.GetOwnedFragmentCount(req.fragmentItemId);
                bool hasSufficient = owned >= req.quantityRequired;

                string color = hasSufficient ? "#00FF00" : "#FF4444";
                string check = hasSufficient ? "✅" : "❌";

                info += $"  {check} <color={color}>{req.displayName}: {owned}/{req.quantityRequired}</color>\n";

                if (!hasSufficient) allMet = false;
            }

            if (allMet) {
                info += "\n<color=#00FF00>Tienes todos los fragmentos!</color>";
            } else {
                info += "\n<color=#FF4444>Te faltan fragmentos. Recoge mas por el mapa.</color>";
            }

            fragmentsInfoText.text = info;
        }

        private void OnConfirm() {
            if (_currentPC == null || HackingManager.Instance == null) return;

            var keys = HackingManager.Instance.GetKeys();
            if (keys.Count == 0) {
                if (errorText != null) errorText.text = "ERROR: No tienes llaves.";
                return;
            }

            int selectedIndex = keyDropdown != null ? keyDropdown.value : 0;
            if (selectedIndex < 0 || selectedIndex >= keys.Count) {
                if (errorText != null) errorText.text = "ERROR: Selecciona una llave valida.";
                return;
            }

            string selectedKey = keys[selectedIndex];
            bool success = _currentPC.TryComplete(selectedKey);

            if (success) {
                HackingManager.Instance.UseKey(selectedKey);

                // quitar llave del inventario tambien
                if (InventoryManager.Instance != null) {
                    InventoryManager.Instance.RemoveItem(4, 1);
                }

                Debug.Log($"PC Corrupto \"{_currentPC.pcName}\" completado!");
                Close();
            } else {
                UpdateFragmentsInfo();
                if (errorText != null) errorText.text = "FALLO: Te faltan fragmentos.";
            }
        }

        private void OnCancel() {
            Close();
        }
    }
}
