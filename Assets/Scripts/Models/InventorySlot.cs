namespace Models {
    [System.Serializable]
    public class InventorySlot {
        public int id;
        public int userId;
        public int itemId;
        public int quantity;
        public int slotIndex; // 0, 1 o 2

        public InventorySlot() {}

        public InventorySlot(int uId, int iId, int qty, int slot) {
            userId = uId;
            itemId = iId;
            quantity = qty;
            slotIndex = slot;
        }

        public bool IsEmpty() {
            return itemId <= 0 || quantity <= 0;
        }
    }
}
