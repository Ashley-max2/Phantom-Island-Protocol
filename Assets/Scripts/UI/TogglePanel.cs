using UnityEngine;

namespace UI {
    public class TogglePanel : MonoBehaviour {

        public GameObject panel;

        public void Toggle() {
            if (panel != null) {
                panel.SetActive(!panel.activeSelf);
            }
        }
    }
}
