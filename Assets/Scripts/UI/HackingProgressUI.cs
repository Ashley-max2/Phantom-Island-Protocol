using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Controllers;

namespace UI {
    // muestra el progreso de hackeo y la pantalla de victoria
    public class HackingProgressUI : MonoBehaviour {
        [Header("Progreso")]
        public TextMeshProUGUI progressText;
        public TextMeshProUGUI keysText;

        [Header("Panel de Victoria")]
        public GameObject victoryPanel;
        public TextMeshProUGUI victoryText;

        void Start() {
            if (victoryPanel != null) victoryPanel.SetActive(false);

            if (HackingManager.Instance != null) {
                HackingManager.Instance.OnProgressChanged += UpdateProgress;
                HackingManager.Instance.OnGameWon += ShowVictoryScreen;
            }

            // leer progreso real de la bd
            if (HackingManager.Instance != null) {
                int hacked = HackingManager.Instance.GetHackedCount();
                int total = HackingManager.Instance.corruptedPCsToWin;
                UpdateProgressText(hacked, total);
            } else {
                UpdateProgressText(0, 3);
            }
            UpdateKeysText();
        }

        void Update() {
            UpdateKeysText();
        }

        void OnDestroy() {
            if (HackingManager.Instance != null) {
                HackingManager.Instance.OnProgressChanged -= UpdateProgress;
                HackingManager.Instance.OnGameWon -= ShowVictoryScreen;
            }
        }

        private void UpdateProgress(int hacked, int total) {
            UpdateProgressText(hacked, total);
        }

        private void UpdateProgressText(int hacked, int total) {
            if (progressText != null) {
                progressText.text = $"PCs Hackeados: {hacked}/{total}";
            }
        }

        private void UpdateKeysText() {
            if (keysText == null || HackingManager.Instance == null) return;

            var keys = HackingManager.Instance.GetKeys();
            if (keys.Count == 0) {
                keysText.text = "Llaves: ninguna";
            } else {
                keysText.text = "Llaves: " + string.Join(", ", keys);
            }
        }

        private void ShowVictoryScreen() {
            if (victoryPanel != null) {
                victoryPanel.SetActive(true);
                Time.timeScale = 0f;
            }

            if (victoryText != null) {
                victoryText.text = "¡MISIÓN COMPLETADA!\n¡La isla es tuya!";
            }
        }
    }
}
