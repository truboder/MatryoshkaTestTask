using System;

using UnityEngine;

using CookingPrototype.Kitchen;
using CookingPrototype.UI;

using JetBrains.Annotations;

namespace CookingPrototype.Controllers {
    public sealed class GameplayController : MonoBehaviour {
        public static GameplayController Instance { get; private set; }

        public GameObject TapBlock   = null;
        public WinWindow  WinWindow  = null;
        public LoseWindow LoseWindow = null;
        public StartWindow StartWindow = null;

        private int _ordersTarget = 0;
        private bool _isInitialized = false;

        public int OrdersTarget {
            get { return _ordersTarget; }
            set {
                _ordersTarget = value;
                if (!_isInitialized) {
                    TotalOrdersServedChanged?.Invoke();
                }
            }
        }

        public int TotalOrdersServed { get; private set; } = 0;

        public event Action TotalOrdersServedChanged;

        void Awake() {
            if (Instance != null) {
                Debug.LogError("Another instance of GameplayController already exists");
            }
            Instance = this;
        }

        void OnDestroy() {
            if (Instance == this) {
                Instance = null;
            }
        }

        void Start() {
            Time.timeScale = 0f;
            if (!_isInitialized) {
                Init();
                CustomersController.Instance.Init();
                _isInitialized = true;
            }
            TotalOrdersServedChanged?.Invoke();
            StartWindow?.Show();
        }

        void Init() {
            TotalOrdersServed = 0;
            TotalOrdersServedChanged?.Invoke();
        }

        public void CheckGameFinish() {
            if (CustomersController.Instance.IsComplete) {
                EndGame(TotalOrdersServed >= OrdersTarget);
            }
        }

        void EndGame(bool win) {
            Time.timeScale = 0f;
            TapBlock?.SetActive(true);
            if (win) {
                WinWindow.Show();
            } else {
                LoseWindow.Show();
            }
        }

        void HideWindows() {
            TapBlock?.SetActive(false);
            WinWindow?.Hide();
            LoseWindow?.Hide();
            StartWindow?.Hide();
        }

        [UsedImplicitly]
        public bool TryServeOrder(Order order) {
            if (!CustomersController.Instance.ServeOrder(order)) {
                return false;
            }

            TotalOrdersServed++;
            TotalOrdersServedChanged?.Invoke();
            CheckGameFinish();
            return true;
        }

        public void Restart() {
            Init();
            CustomersController.Instance.ResetState();
            HideWindows();

            foreach (var place in FindObjectsByType<AbstractFoodPlace>(FindObjectsSortMode.None)) {
                place.FreePlace();
            }
        }

        public void CloseGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}