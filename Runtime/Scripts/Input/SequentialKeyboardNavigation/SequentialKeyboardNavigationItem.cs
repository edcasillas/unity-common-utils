using CommonUtils.Input.ButtonExternalControllers;
using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.Input.SequentialKeyboardNavigation {
    public class SequentialKeyboardNavigationItem : ButtonFromKeyboard, ISequentialKeyboardNavigationItem {
        [SerializeField] private int tabIndex;
        [SerializeField] private KeyCode keyCodeWhenFocused = KeyCode.Return;

        public int TabIndex => tabIndex;

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

        public void SetFocus(bool hasFocus) {
            // TODO Activate animation
        }
    }
}