using CommonUtils.Input.ButtonExternalControllers;
using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.Input.SequentialKeyboardNavigation {
    public class FocusableButtonFromKeyboard : ButtonFromKeyboard, IFocusableButtonFromKeyboard {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
        [SerializeField] private int tabIndex;
        [SerializeField] private KeyCode keyCodeWhenFocused = KeyCode.Return;

        public int TabIndex => tabIndex;

        private bool _hasFocus;

        public bool HasFocus {
            get => _hasFocus;
            set {
                // TODO Activate feedback
            }
        }

        private ISequentialKeyboardNavigationManager _manager;
        private ISequentialKeyboardNavigationManager manager {
            get {
                if(!_manager.IsValid()) _manager = SequentialKeyboardNavigationManager.Instance;
                return _manager;
            }
            set => _manager = value;
        }
        
        #region Unity Lifecycle
        protected override void Awake() {
            base.Awake();
            manager.Subscribe(this);
        }

        private void OnEnable() => manager.OnItemEnabledOrDisabled();

        private void OnDisable() => manager.OnItemEnabledOrDisabled();

        private void OnDestroy() => manager.Unsubscribe(this);
        #endregion

        protected override bool IsKeyPressed() => (KeyCode != KeyCode.None && base.IsKeyPressed()) || (HasFocus && UnityEngine.Input.GetKeyDown(keyCodeWhenFocused));
        protected override bool IsKeyReleased() => (KeyCode != KeyCode.None && base.IsKeyReleased()) || (HasFocus && UnityEngine.Input.GetKeyUp(keyCodeWhenFocused));
#else
        public int TabIndex => 0;
        public bool HasFocus { get; set; }
        bool IsInteractable() => false;
#endif
    }
}