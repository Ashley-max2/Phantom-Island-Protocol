using System.Collections.Generic;
using UnityEngine;
using Database;

namespace Controllers {
    // gestiona las llaves y el progreso de hackeo
    public class HackingManager : MonoBehaviour {
        public static HackingManager Instance;

        public int corruptedPCsToWin = 3;

        [SerializeField] private int _hackedPCCount = 0;
        [SerializeField] private List<string> _keys = new List<string>();

        public System.Action<int, int> OnProgressChanged;
        public System.Action OnGameWon;

        private ProgressRepository _progressRepo;

        void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            _progressRepo = new ProgressRepository();
        }

        void Start() {
            LoadProgress();
        }

        // carga llaves y pcs hackeados de la bd
        public void LoadProgress() {
            if (GameManager.Instance == null || GameManager.Instance.CurrentUser == null) return;

            int userId = GameManager.Instance.CurrentUser.id;

            _keys = _progressRepo.LoadKeys(userId);
            Debug.Log($"[HackingManager] Llaves cargadas: {_keys.Count}");

            List<string> hackedPCs = _progressRepo.LoadHackedPCs(userId);
            _hackedPCCount = hackedPCs.Count;
            Debug.Log($"[HackingManager] PCs hackeados cargados: {_hackedPCCount}/{corruptedPCsToWin}");

            OnProgressChanged?.Invoke(_hackedPCCount, corruptedPCsToWin);
        }

        // añade una llave y la guarda en bd
        public void AddKey(string keyName) {
            _keys.Add(keyName);
            Debug.Log($"[HackingManager] Llave añadida: \"{keyName}\". Total: {_keys.Count}");

            if (GameManager.Instance?.CurrentUser != null) {
                _progressRepo.SaveKey(GameManager.Instance.CurrentUser.id, keyName);
            }
        }

        public bool HasKey(string keyName) {
            return _keys.Contains(keyName);
        }

        // usa y borra una llave
        public bool UseKey(string keyName) {
            if (_keys.Contains(keyName)) {
                _keys.Remove(keyName);
                Debug.Log($"[HackingManager] Llave \"{keyName}\" consumida.");

                if (GameManager.Instance?.CurrentUser != null) {
                    _progressRepo.DeleteKey(GameManager.Instance.CurrentUser.id, keyName);
                }
                return true;
            }
            return false;
        }

        public List<string> GetKeys() {
            return new List<string>(_keys);
        }

        public int KeyCount() {
            return _keys.Count;
        }

        // registra un pc como hackeado
        public void RegisterHackedPC(string pcName) {
            _hackedPCCount++;
            Debug.Log($"[HackingManager] PC hackeado: {pcName}. Progreso: {_hackedPCCount}/{corruptedPCsToWin}");

            if (GameManager.Instance?.CurrentUser != null) {
                _progressRepo.SaveHackedPC(GameManager.Instance.CurrentUser.id, pcName);
            }

            OnProgressChanged?.Invoke(_hackedPCCount, corruptedPCsToWin);

            if (_hackedPCCount >= corruptedPCsToWin) {
                WinGame();
            }
        }

        // comprueba si un pc ya fue hackeado antes
        public bool IsPCAlreadyHacked(string pcName) {
            if (GameManager.Instance?.CurrentUser == null) return false;
            return _progressRepo.IsPCHacked(GameManager.Instance.CurrentUser.id, pcName);
        }

        public int GetHackedCount() {
            return _hackedPCCount;
        }

        private void WinGame() {
            Debug.Log("VICTORIA! Has hackeado todos los PCs corruptos.");
            OnGameWon?.Invoke();

            if (GameManager.Instance != null) {
                GameManager.Instance.isGameActive = false;
            }
        }
    }
}
