using UnityEngine;
using Models;

namespace Controllers {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance;
        public User CurrentUser { get; private set; }
        public float timeRemaining = 900f; // 15 min
        public bool isGameActive = false;

        void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }

        void Update() {
            if (isGameActive && timeRemaining > 0) {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0) {
                    GameOver();
                }
            }
        }

        public void SetUser(User user) {
            CurrentUser = user;
            Debug.Log($"Session started for: {user.username}");
        }

        public void StartGame() {
            isGameActive = true;
        }

        private void GameOver() {
            isGameActive = false;
            Debug.Log("Mission Failed: Time expired.");
        }

        public void GameWon() {
            isGameActive = false;
            Debug.Log("MISION COMPLETADA!");
        }
    }
}
