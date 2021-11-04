using CommonUtils.UnityComponents;

namespace CommonUtils.Input.SequentialKeyboardNavigation {
    public interface ISequentialKeyboardNavigationItem : IUnityComponent{
        int TabIndex { get; }

        void SetFocus(bool hasFocus);
        bool IsInteractable();
    }
}