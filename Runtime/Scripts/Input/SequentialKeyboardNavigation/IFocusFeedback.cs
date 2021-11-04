using CommonUtils.UnityComponents;

namespace CommonUtils.Input.SequentialKeyboardNavigation {
    public interface IFocusFeedback : IUnityComponent {
        void SetFocus(bool hasFocus);
    }
}