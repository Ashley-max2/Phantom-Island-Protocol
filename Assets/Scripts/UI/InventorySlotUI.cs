using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Models;
using Controllers;

namespace UI {
    // cada slot del inventario (icono, cantidad, boton tirar)
    public class InventorySlotUI : MonoBehaviour {
        public Image icon;
        public TextMeshProUGUI quantityText;
        public TextMeshProUGUI nameText;
        public Button dropButton;
        public GameObject emptyState;
        public GameObject filledState;

        public Sprite emptySlotSprite;

        private int _slotIndex;

        void Awake() {
            if (dropButton != null) {
                dropButton.onClick.AddListener(OnDropClicked);
            }
        }

        public void SetSlotIndex(int index) {
            _slotIndex = index;
        }

        // actualiza la visual del slot
        public void Setup(Item item, int quantity) {
            if (item != null && quantity > 0) {
                ShowFilled(true);

                if (icon != null) {
                    Sprite sprite = null;
                    if (InventoryManager.Instance != null) {
                        sprite = InventoryManager.Instance.GetItemSprite(item.id);
                    }
                    if (sprite == null && !string.IsNullOrEmpty(item.iconPath)) {
                        sprite = Resources.Load<Sprite>(item.iconPath);
                    }
                    if (sprite != null) {
                        icon.sprite = sprite;
                        icon.color = Color.white;
                    }
                }

                if (nameText != null) nameText.text = item.itemName;

                if (quantityText != null) {
                    if (quantity > 1) {
                        quantityText.text = "x" + quantity.ToString();
                        quantityText.gameObject.SetActive(true);
                    } else {
                        quantityText.text = "";
                        quantityText.gameObject.SetActive(false);
                    }
                }

                if (dropButton != null) dropButton.gameObject.SetActive(true);

            } else {
                Clear();
            }
        }

        // deja el slot vacio
        public void Clear() {
            ShowFilled(false);

            if (icon != null) {
                icon.sprite = emptySlotSprite;
                icon.color = new Color(1f, 1f, 1f, 0.2f);
            }
            if (nameText != null) nameText.text = "";
            if (quantityText != null) {
                quantityText.text = "";
                quantityText.gameObject.SetActive(false);
            }
            if (dropButton != null) dropButton.gameObject.SetActive(false);
        }

        private void OnDropClicked() {
            if (InventoryManager.Instance != null) {
                InventoryManager.Instance.DropSlot(_slotIndex);
            }
        }

        private void ShowFilled(bool filled) {
            if (emptyState != null) emptyState.SetActive(!filled);
            if (filledState != null) filledState.SetActive(filled);
        }
    }
}
