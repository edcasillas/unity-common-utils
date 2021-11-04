using CommonUtils.UnityComponents;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation.FocusFeedback {
    public interface IFocusFeedback : IUnityComponent {
        void SetFocus(bool hasFocus);
    }
}