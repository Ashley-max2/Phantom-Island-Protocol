using System.Collections;
using UnityEngine;
using TMPro;
using Controllers;
using Models;

namespace UI {
    // panel del inventario con los 3 slots fijos
    public class InventoryUI : MonoBehaviour {
        [Header("Los 3 Slots")]
        public InventorySlotUI slot0;
        public InventorySlotUI slot1;
        public InventorySlotUI slot2;

        [Header("Mensaje inventario lleno")]
        public TextMeshProUGUI fullMessage;
        public float messageDuration = 2f;

        private InventorySlotUI[] _slotUIs;

        void Awake() {
            _slotUIs = new InventorySlotUI[] { slot0, slot1, slot2 };

            for (int i = 0; i < _slotUIs.Length; i++) {
                if (_slotUIs[i] != null) {
                    _slotUIs[i].SetSlotIndex(i);
                }
            }

            if (fullMessage != null) fullMessage.gameObject.SetActive(false);
        }

        void Start() {
            if (InventoryManager.Instance != null) {
                InventoryManager.Instance.ui = this;
            }
        }

        // actualiza los 3 slots con los datos del inventario
        public void UpdateSlots(InventorySlot[] slots) {
            if (slots == null) return;

            InventoryManager mgr = InventoryManager.Instance;

            for (int i = 0; i < _slotUIs.Length; i++) {
                if (_slotUIs[i] == null) continue;

                if (i < slots.Length && slots[i] != null) {
                    Item itemDef = null;
                    if (mgr != null) {
                        itemDef = mgr.GetItemInfo(slots[i].itemId);
                    }
                    _slotUIs[i].Setup(itemDef, slots[i].quantity);
                } else {
                    _slotUIs[i].Clear();
                }
            }
        }

        public void ShowFullMessage() {
            if (fullMessage != null) {
                fullMessage.text = "Inventario lleno! Tira algo primero.";
                fullMessage.gameObject.SetActive(true);
                StopAllCoroutines();
                StartCoroutine(HideMessageAfterDelay());
            }
        }

        private IEnumerator HideMessageAfterDelay() {
            yield return new WaitForSeconds(messageDuration);
            if (fullMessage != null) fullMessage.gameObject.SetActive(false);
        }
    }
}
