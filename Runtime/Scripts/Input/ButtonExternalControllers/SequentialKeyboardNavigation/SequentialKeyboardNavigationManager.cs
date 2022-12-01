using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using CommonUtils.UnityComponents;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation {
    public class SequentialKeyboardNavigationManager : MonoBehaviour, ISequentialKeyboardNavigationManager, IVerbosable {
        #region Singleton
        private static ISequentialKeyboardNavigationManager _instance;
        public static ISequentialKeyboardNavigationManager Instance {
            get {
                if (!_instance.IsValid()) {
                    _instance = FindObjectOfType<SequentialKeyboardNavigationManager>();
                    if (!_instance.IsValid()) {
                        var go = new GameObject(nameof(SequentialKeyboardNavigationManager));
                        _instance = go.AddComponent<SequentialKeyboardNavigationManager>();
                    }
                }

                return _instance;
            }
        }
        #endregion

        #region Inspector fields
        [SerializeField] private KeyCode navigationKey = KeyCode.Tab;
        [SerializeField] private bool verbose;
        #endregion

        #region Properties and backing fields
        private readonly SortedSet<IFocusableButtonFromKeyboard> _allItems = new SortedSet<IFocusableButtonFromKeyboard>(new FocusableButtonFromKeyboardComparer());
        [ShowInInspector] public IEnumerable<IFocusableButtonFromKeyboard> AllItems => _allItems;

        private List<IFocusableButtonFromKeyboard> _currentlyActiveItems = new List<IFocusableButtonFromKeyboard>();
        [ShowInInspector] public IEnumerable<IFocusableButtonFromKeyboard> CurrentlyActiveItems => _currentlyActiveItems;

        private int? currentIndex = null;
        [ShowInInspector] public int CurrentIndex => currentIndex ?? -1;

        [ShowInInspector] public IFocusableButtonFromKeyboard CurrentlyFocusedItem { get; private set; }

        public bool IsVerbose => verbose;

        #endregion

        #region Fields
        private bool needsRefreshOfCurrentlyActiveItems;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            if (_instance.IsValid() && (SequentialKeyboardNavigationManager)_instance != this) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += onSceneLoaded;
        }

        private void Update() {
            if (needsRefreshOfCurrentlyActiveItems) {
                refreshCurrentlyActiveItems();
                needsRefreshOfCurrentlyActiveItems = false;
            }

            if (_currentlyActiveItems.Count == 0) return;

            if (UnityEngine.Input.GetKeyUp(navigationKey)) {
                if (!currentIndex.HasValue) currentIndex = 0;
                else currentIndex++;

                currentIndex %= _currentlyActiveItems.Count;

                if(CurrentlyFocusedItem.IsValid()) CurrentlyFocusedItem.HasFocus = false;

                CurrentlyFocusedItem = _currentlyActiveItems[currentIndex.Value];

                if(CurrentlyFocusedItem.IsValid()) CurrentlyFocusedItem.HasFocus = true;
            }
        }

        private void OnDestroy() {
            if ((SequentialKeyboardNavigationManager)_instance == this) _instance = null;
        }

        private void OnApplicationQuit() => Destroy(gameObject);
        #endregion

        #region Public Methods
        public void Subscribe(IFocusableButtonFromKeyboard item) {
            _allItems.Add(item);
            needsRefreshOfCurrentlyActiveItems = true;
            this.DebugLog($"{item.name} has been SUBSCRIBED to {name}");
        }

        public void Unsubscribe(IFocusableButtonFromKeyboard item) {
            _allItems.Remove(item);
            needsRefreshOfCurrentlyActiveItems = true;
            this.DebugLog($"{item.name} has been UNSUBSCRIBED from {name}");
        }

        [ShowInInspector] public void OnItemEnabledOrDisabled() => needsRefreshOfCurrentlyActiveItems = true;
        #endregion

        #region Private methods
        private void onSceneLoaded(Scene arg0, LoadSceneMode arg1) {
            var removedItems = _allItems.RemoveWhere(item => !item.IsValid());
            this.DebugLog($"A new scene was loaded and {removedItems} items were removed from {name}");
            refreshCurrentlyActiveItems();
        }

        private void refreshCurrentlyActiveItems() {
            _currentlyActiveItems = _allItems.Where(i => i.IsInteractable()).ToList();
            if (CurrentlyFocusedItem.IsValid() && !_currentlyActiveItems.Contains(CurrentlyFocusedItem)) {
                CurrentlyFocusedItem.HasFocus = false;
                CurrentlyFocusedItem = null;
            }
            currentIndex = null;
        }
        #endregion
    }
}
