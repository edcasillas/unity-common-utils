using CommonUtils.UnityComponents;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation {
    public interface IFocusableButtonFromKeyboard : IButtonExternalController {
        int TabIndex { get; set; }

        bool HasFocus { get; set; }
    }
}