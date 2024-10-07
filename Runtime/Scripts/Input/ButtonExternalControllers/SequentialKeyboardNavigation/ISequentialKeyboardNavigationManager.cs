using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation {
    public interface ISequentialKeyboardNavigationManager : IUnityComponent, IVerbosable {
        void Subscribe(IFocusableButtonFromKeyboard item);
        void Unsubscribe(IFocusableButtonFromKeyboard item);
        void OnItemEnabledOrDisabled();
    }
}