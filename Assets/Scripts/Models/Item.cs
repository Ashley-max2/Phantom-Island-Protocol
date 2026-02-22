using UnityEngine;

namespace Models {
    [System.Serializable]
    public class Item {
        public int id;
        public string itemName;
        public string description;
        public int maxStack;    // 1 = no se acumula, >1 = se acumula
        public string iconPath; // ruta en Resources

        public bool IsStackable() {
            return maxStack > 1;
        }
    }
}
