using UnityEngine;
using Controllers;

namespace World {
    public class ItemPickup : MonoBehaviour {
        public int itemId; 
        public int quantity = 1;

        public Sprite itemSprite;

        void Start() {
            // poner el sprite al objeto del mundo
            if (itemSprite != null) {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null) {
                    sr.sprite = itemSprite;
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                PickUp();
            }
        }

        void PickUp() {
            if (InventoryManager.Instance == null) return;

            bool picked = InventoryManager.Instance.AddItem(itemId, quantity);
            
            if (picked) {
                Destroy(gameObject);
            }
        }
    }
}
