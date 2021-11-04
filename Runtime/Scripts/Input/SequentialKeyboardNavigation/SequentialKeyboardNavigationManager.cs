using System.Collections.Generic;
using System.Linq;
using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.Input.SequentialKeyboardNavigation {
    public class SequentialKeyboardNavigationManager : MonoBehaviour, ISequentialKeyboardNavigationManager {
        #region Static members

        private static readonly SortedSet<IFocusableButtonFromKeyboard> allItems =
            new SortedSet<IFocusableButtonFromKeyboard>(new FocusableButtonFromKeyboardComparer());
        
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

        private static List<IFocusableButtonFromKeyboard> currentlyActiveItems =
            new List<IFocusableButtonFromKeyboard>();

        private static int? currentIndex = null;

        private static IFocusableButtonFromKeyboard currentlyFocusedItem;

        private static void updateCurrentlyActiveItems() {
            currentlyActiveItems = allItems.Where(i => i.IsInteractable()).ToList();
            if (currentlyFocusedItem.IsValid() && !currentlyActiveItems.Contains(currentlyFocusedItem)) {
                currentlyFocusedItem.HasFocus = false;
                currentlyFocusedItem = null;
            }
            currentIndex = null;
        }
        #endregion

        [SerializeField] private KeyCode navigationKey = KeyCode.Tab;

        #region Unity Lifecycle
        private void Awake() {
            if (_instance.IsValid() && (SequentialKeyboardNavigationManager)_instance != this) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            if (currentlyActiveItems.Count == 0) return;
            
            if (UnityEngine.Input.GetKeyUp(navigationKey)) {
                if (!currentIndex.HasValue) currentIndex = 0;
                else currentIndex++;

                currentIndex %= currentlyActiveItems.Count;
                
                if(currentlyFocusedItem.IsValid()) currentlyFocusedItem.HasFocus = false;

                currentlyFocusedItem = currentlyActiveItems[currentIndex.Value];
                
                if(currentlyFocusedItem.IsValid()) currentlyFocusedItem.HasFocus = true;
            }
        }

        private void OnDestroy() {
            if ((SequentialKeyboardNavigationManager)_instance == this) _instance = null;
        }
        #endregion
        
        #region Public Methods
        public void Subscribe(IFocusableButtonFromKeyboard item) {
            allItems.Add(item);
            updateCurrentlyActiveItems();
        }

        public void Unsubscribe(IFocusableButtonFromKeyboard item) {
            allItems.Remove(item);
            updateCurrentlyActiveItems();
        }

        public void OnItemEnabledOrDisabled() {
            updateCurrentlyActiveItems();
        }
        #endregion
    }
}
