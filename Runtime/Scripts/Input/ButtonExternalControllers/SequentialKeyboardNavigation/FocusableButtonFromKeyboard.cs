using CommonUtils.Extensions;
using CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation.FocusFeedback;
using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;
using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation {
    public class FocusableButtonFromKeyboard : ButtonFromKeyboard, IFocusableButtonFromKeyboard {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
        #region Inspector fields
        [SerializeField] private int tabIndex;
        [SerializeField] private KeyCode keyCodeWhenFocused = KeyCode.Return;
        #endregion

        #region Properties and backing fields
        public int TabIndex {
            get => tabIndex;
            set => tabIndex = value;
        }

        private bool _hasFocus;
        public bool HasFocus {
            get => _hasFocus;
            set {
                _hasFocus = value;
                if(focusFeedback.IsValid()) focusFeedback.SetFocus(_hasFocus);
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

        private IFocusFeedback _focusFeedback;
        private IFocusFeedback focusFeedback {
            get {
                if (!_focusFeedback.IsValid()) _focusFeedback = GetComponent<IFocusFeedback>();
                return _focusFeedback;
            }
        }
        #endregion

        #region Unity Lifecycle
        protected override void Awake() {
            base.Awake();
            manager.Subscribe(this);
        }

        private void OnEnable() {
            this.Log($"{name} has been enabled.");
            manager.OnItemEnabledOrDisabled();
        }

        private void OnDisable() {
            this.Log($"{name} has been disabled.");
            manager.OnItemEnabledOrDisabled();
        }

        private void OnDestroy() {
            if(manager.IsValid()) manager.Unsubscribe(this);
        }
        #endregion

        #region Overrides
        protected override bool IsKeyPressed() => (KeyCode != KeyCode.None && base.IsKeyPressed()) || (HasFocus && UnityEngine.Input.GetKeyDown(keyCodeWhenFocused));
        protected override bool IsKeyReleased() => (KeyCode != KeyCode.None && base.IsKeyReleased()) || (HasFocus && UnityEngine.Input.GetKeyUp(keyCodeWhenFocused));
        protected override void OnBlockersChanged() => manager.OnItemEnabledOrDisabled();

        public override void OnBlockerBecameActive(IButtonExternalControllerBlocker blocker) {
            base.OnBlockerBecameActive(blocker);
            manager.OnItemEnabledOrDisabled();
        }

        public override void OnBlockerBecameInactive(IButtonExternalControllerBlocker blocker) {
            base.OnBlockerBecameInactive(blocker);
            manager.OnItemEnabledOrDisabled();
        }

        #endregion
#else
        public int TabIndex { get; set; }
        public bool HasFocus { get; set; }
#endif
    }
}