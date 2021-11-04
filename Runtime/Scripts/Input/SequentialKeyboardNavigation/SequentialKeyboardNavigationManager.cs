using System.Collections.Generic;
using System.Linq;
using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.Input.SequentialKeyboardNavigation {
    public class SequentialKeyboardNavigationManager : MonoBehaviour, ISequentialKeyboardNavigationManager {
        #region Static members

        private static readonly SortedSet<ISequentialKeyboardNavigationItem> allItems =
            new SortedSet<ISequentialKeyboardNavigationItem>(new SequentialKeyboardNavigationItemComparer());
        
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

        private static List<ISequentialKeyboardNavigationItem> currentlyActiveItems =
            new List<ISequentialKeyboardNavigationItem>();

        private static int? currentIndex = null;

        private static ISequentialKeyboardNavigationItem currentlyFocusedItem;

        private static void updateCurrentlyActiveItems() {
            currentlyActiveItems = allItems.Where(i => i.IsInteractable()).ToList();
            if (currentlyFocusedItem.IsValid() && !currentlyActiveItems.Contains(currentlyFocusedItem)) {
                currentlyFocusedItem.SetFocus(false);
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
                
                if(currentlyFocusedItem.IsValid()) currentlyFocusedItem.SetFocus(false);

                currentlyFocusedItem = currentlyActiveItems[currentIndex.Value];
                
                if(currentlyFocusedItem.IsValid()) currentlyFocusedItem.SetFocus(true);
            }
        }

        private void OnDestroy() {
            if ((SequentialKeyboardNavigationManager)_instance == this) _instance = null;
        }
        #endregion
        
        #region Public Methods
        public void Subscribe(ISequentialKeyboardNavigationItem item) {
            allItems.Add(item);
            updateCurrentlyActiveItems();
        }

        public void Unsubscribe(ISequentialKeyboardNavigationItem item) {
            allItems.Remove(item);
            updateCurrentlyActiveItems();
        }

        public void OnItemEnabledOrDisabled() {
            updateCurrentlyActiveItems();
        }
        #endregion
    }
}
