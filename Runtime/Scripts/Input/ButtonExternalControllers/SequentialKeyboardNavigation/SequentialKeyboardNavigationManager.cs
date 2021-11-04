using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation {
    public class SequentialKeyboardNavigationManager : MonoBehaviour, ISequentialKeyboardNavigationManager {
        #region Static members
        private static readonly SortedSet<IFocusableButtonFromKeyboard> allItems =
            new SortedSet<IFocusableButtonFromKeyboard>(new FocusableButtonFromKeyboardComparer());

        public static IEnumerable<IFocusableButtonFromKeyboard> AllItems => allItems;
        
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

        public static IEnumerable<IFocusableButtonFromKeyboard> CurrentlyActiveItems => currentlyActiveItems;

        private static int? currentIndex = null;
        public static int CurrentIndex => currentIndex ?? -1;

        public static IFocusableButtonFromKeyboard CurrentlyFocusedItem { get; private set; }

        private static void updateCurrentlyActiveItems() {
            currentlyActiveItems = allItems.Where(i => i.IsInteractable()).ToList();
            if (CurrentlyFocusedItem.IsValid() && !currentlyActiveItems.Contains(CurrentlyFocusedItem)) {
                CurrentlyFocusedItem.HasFocus = false;
                CurrentlyFocusedItem = null;
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
                
                if(CurrentlyFocusedItem.IsValid()) CurrentlyFocusedItem.HasFocus = false;

                CurrentlyFocusedItem = currentlyActiveItems[currentIndex.Value];
                
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
            allItems.Add(item);
            updateCurrentlyActiveItems();
        }

        public void Unsubscribe(IFocusableButtonFromKeyboard item) {
            allItems.Remove(item);
            updateCurrentlyActiveItems();
        }

        public void OnItemEnabledOrDisabled() => updateCurrentlyActiveItems();
        #endregion
    }
}
