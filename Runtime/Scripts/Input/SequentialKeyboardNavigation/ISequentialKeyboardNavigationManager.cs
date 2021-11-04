using CommonUtils.UnityComponents;

namespace CommonUtils.Input.SequentialKeyboardNavigation {
    public interface ISequentialKeyboardNavigationManager : IUnityComponent {
        void Subscribe(IFocusableButtonFromKeyboard item);
        void Unsubscribe(IFocusableButtonFromKeyboard item);
        void OnItemEnabledOrDisabled();
    }
}