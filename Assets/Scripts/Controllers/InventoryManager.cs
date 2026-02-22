using System.Collections.Generic;
using UnityEngine;
using Database;
using Models;
using UI;

namespace Controllers {
    public class InventoryManager : MonoBehaviour {
        public static InventoryManager Instance;
        public InventoryUI ui;
        
        public const int MAX_SLOTS = 3;

        [Header("Sprites de los items")]
        public Sprite spriteLaptop;
        public Sprite spriteFragmento;
        public Sprite spriteFragmentoHielo;
        public Sprite spriteLlave;

        private InventoryRepository _repo;
        private ItemLogRepository _logRepo;
        private InventorySlot[] _slots = new InventorySlot[MAX_SLOTS];

        // devuelve el sprite segun el id del item
        public Sprite GetItemSprite(int itemId) {
            switch (itemId) {
                case 1: return spriteLaptop;
                case 2: return spriteFragmento;
                case 3: return spriteFragmentoHielo;
                case 4: return spriteLlave;
                default: return null;
            }
        }

        void Awake() {
            if (Instance == null) Instance = this;
            _repo = new InventoryRepository();
            _logRepo = new ItemLogRepository();
        }

        void Start() {
            if (GameManager.Instance != null && GameManager.Instance.CurrentUser != null) {
                LoadInventory();
            }
        }

        // carga el inventario del user desde la bd
        public void LoadInventory() {
            if (GameManager.Instance == null || GameManager.Instance.CurrentUser == null) return;

            for (int i = 0; i < MAX_SLOTS; i++) {
                _slots[i] = null;
            }

            int userId = GameManager.Instance.CurrentUser.id;
            List<InventorySlot> dbSlots = _repo.LoadInventory(userId);

            foreach (var slot in dbSlots) {
                if (slot.slotIndex >= 0 && slot.slotIndex < MAX_SLOTS) {
                    _slots[slot.slotIndex] = slot;
                }
            }

            RefreshUI();
        }

        // intenta meter un item, si el stack esta lleno pasa al siguiente slot
        public bool AddItem(int itemId, int quantity) {
            if (GameManager.Instance == null || GameManager.Instance.CurrentUser == null) {
                Debug.LogWarning("InventoryManager.AddItem: no hay user.");
                return false;
            }

            int userId = GameManager.Instance.CurrentUser.id;
            Item itemDef = _repo.GetItemDefinition(itemId);
            
            if (itemDef == null) {
                Debug.LogWarning($"Item con ID {itemId} no existe.");
                return false;
            }

            int remaining = quantity;

            // primero llenar stacks que ya tengan ese item
            if (itemDef.IsStackable()) {
                for (int i = 0; i < MAX_SLOTS && remaining > 0; i++) {
                    if (_slots[i] != null && _slots[i].itemId == itemId) {
                        int spaceLeft = itemDef.maxStack - _slots[i].quantity;
                        if (spaceLeft > 0) {
                            int toAdd = Mathf.Min(remaining, spaceLeft);
                            _repo.AddItem(userId, itemId, toAdd, i);
                            remaining -= toAdd;
                        }
                    }
                }
            }

            // despues buscar slots vacios
            while (remaining > 0) {
                int emptySlot = -1;
                for (int i = 0; i < MAX_SLOTS; i++) {
                    if (_slots[i] == null) {
                        emptySlot = i;
                        break;
                    }
                }

                if (emptySlot == -1) break;

                int toPlace = itemDef.IsStackable() ? Mathf.Min(remaining, itemDef.maxStack) : remaining;
                _repo.AddItem(userId, itemId, toPlace, emptySlot);
                remaining -= toPlace;

                _slots[emptySlot] = new InventorySlot(userId, itemId, toPlace, emptySlot);
            }

            if (remaining < quantity) {
                int placed = quantity - remaining;
                if (GameManager.Instance?.CurrentUser != null) {
                    _logRepo.LogAction(GameManager.Instance.CurrentUser.id, itemId, "RECOGER", placed);
                }
                LoadInventory();
                return true;
            }

            // inventario lleno
            Debug.Log("Inventario lleno! Tira algo primero.");
            if (ui != null) {
                ui.ShowFullMessage();
            }
            return false;
        }

        // tira todo el contenido de un slot
        public void DropSlot(int slotIndex) {
            if (GameManager.Instance == null || GameManager.Instance.CurrentUser == null) return;
            if (slotIndex < 0 || slotIndex >= MAX_SLOTS) return;
            if (_slots[slotIndex] == null) return;

            int userId = GameManager.Instance.CurrentUser.id;
            int droppedItemId = _slots[slotIndex].itemId;
            int droppedQty = _slots[slotIndex].quantity;
            
            Debug.Log($"Tiraste: {GetItemInfo(droppedItemId)?.itemName ?? "item"} del slot {slotIndex + 1}");
            
            _logRepo.LogAction(userId, droppedItemId, "TIRAR", droppedQty);
            
            _repo.ClearSlot(userId, slotIndex);
            _slots[slotIndex] = null;
            RefreshUI();
        }

        // quita X cantidad de un item (para consumir fragmentos, llaves etc)
        public void RemoveItem(int itemId, int quantity) {
            if (GameManager.Instance == null || GameManager.Instance.CurrentUser == null) return;

            int userId = GameManager.Instance.CurrentUser.id;
            _logRepo.LogAction(userId, itemId, "USAR", quantity);
            _repo.RemoveItem(userId, itemId, quantity);
            LoadInventory();
        }

        public Item GetItemInfo(int itemId) {
            return _repo.GetItemDefinition(itemId);
        }

        public bool HasItem(int itemId) {
            for (int i = 0; i < MAX_SLOTS; i++) {
                if (_slots[i] != null && _slots[i].itemId == itemId && _slots[i].quantity > 0) {
                    return true;
                }
            }
            return false;
        }

        public InventorySlot GetSlot(int index) {
            if (index < 0 || index >= MAX_SLOTS) return null;
            return _slots[index];
        }

        private void RefreshUI() {
            if (ui != null) {
                ui.UpdateSlots(_slots);
            }
        }
    }
}
