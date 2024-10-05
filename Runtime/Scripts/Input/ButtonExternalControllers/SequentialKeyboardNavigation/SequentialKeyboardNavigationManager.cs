using System.Collections.Generic;
using System.Linq;
using CommonUtils.Logging;
using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;
using UnityEngine;
using UnityEngine.SceneManagement;
using ILogger = CommonUtils.Logging.ILogger;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation {
    public class SequentialKeyboardNavigationManager : EnhancedMonoBehaviour, ISequentialKeyboardNavigationManager {
        #region Singleton
		private static bool instanceHasBeenResolved = false;
        private static ISequentialKeyboardNavigationManager instance;
        public static ISequentialKeyboardNavigationManager Instance {
            get {
                if (!instance.IsValid()) {
					if (instanceHasBeenResolved) {
						if (SingletonRegistry.TryResolve<ILogger>(out var logger)) {
							logger.Log(LogLevel.Error, $"Instance of {nameof(SequentialKeyboardNavigationManager)} has already been destroyed. This might happen when the application is being terminated.");
						}
						return null;
					}

                    instance = FindObjectOfType<SequentialKeyboardNavigationManager>();
					if(instance.IsValid()) instance.Log2($"{nameof(SequentialKeyboardNavigationManager)} found in scene.", LogLevel.Warning);

                    if (!instance.IsValid()) {
                        var go = new GameObject(nameof(SequentialKeyboardNavigationManager));
                        instance = go.AddComponent<SequentialKeyboardNavigationManager>();
						if(instance.IsValid()) instance.Log2($"{nameof(SequentialKeyboardNavigationManager)} not found in scene, a game object has been added to the scene.", LogLevel.Warning);
                    }
					if(instance.IsValid()) instanceHasBeenResolved = true;
                }

                return instance;
            }
        }
        #endregion

        #region Inspector fields
        [SerializeField] private KeyCode navigationKey = KeyCode.Tab;
        #endregion

        #region Properties and backing fields
        private readonly SortedSet<IFocusableButtonFromKeyboard> _allItems = new SortedSet<IFocusableButtonFromKeyboard>(new FocusableButtonFromKeyboardComparer());
        [ShowInInspector] public IEnumerable<IFocusableButtonFromKeyboard> AllItems => _allItems;

        private List<IFocusableButtonFromKeyboard> _currentlyActiveItems = new List<IFocusableButtonFromKeyboard>();
        [ShowInInspector] public IEnumerable<IFocusableButtonFromKeyboard> CurrentlyActiveItems => _currentlyActiveItems;

        private int? currentIndex = null;
        [ShowInInspector] public int CurrentIndex => currentIndex ?? -1;

        [ShowInInspector] public IFocusableButtonFromKeyboard CurrentlyFocusedItem { get; private set; }
        #endregion

        #region Fields
        private bool needsRefreshOfCurrentlyActiveItems;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            if (instance.IsValid() && (SequentialKeyboardNavigationManager)instance != this) {
				this.Log($"Multiple instances of {nameof(SequentialKeyboardNavigationManager)} are active. Instance is {instance.name}.", LogLevel.Warning);

                Destroy(gameObject);
                return;
            }

            instance = this;
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
            if ((SequentialKeyboardNavigationManager)instance == this) instance = null;
        }

        private void OnApplicationQuit() => Destroy(gameObject);
        #endregion

        #region Public Methods
        public void Subscribe(IFocusableButtonFromKeyboard item) {
            _allItems.Add(item);
            needsRefreshOfCurrentlyActiveItems = true;
            this.Log($"{item.name} has been SUBSCRIBED to {name}");
        }

        public void Unsubscribe(IFocusableButtonFromKeyboard item) {
            _allItems.Remove(item);
            needsRefreshOfCurrentlyActiveItems = true;
            this.Log($"{item.name} has been UNSUBSCRIBED from {name}");
        }

        [ShowInInspector] public void OnItemEnabledOrDisabled() => needsRefreshOfCurrentlyActiveItems = true;
        #endregion

        #region Private methods
        private void onSceneLoaded(Scene arg0, LoadSceneMode arg1) {
            var removedItems = _allItems.RemoveWhere(item => !item.IsValid());
            this.Log($"A new scene was loaded and {removedItems} items were removed from {name}");
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
