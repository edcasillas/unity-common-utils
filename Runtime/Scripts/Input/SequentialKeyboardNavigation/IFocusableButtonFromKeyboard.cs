using CommonUtils.UnityComponents;

namespace CommonUtils.Input.SequentialKeyboardNavigation {
    public interface IFocusableButtonFromKeyboard : IUnityComponent{
        int TabIndex { get; }

        bool HasFocus { get; set; }
        bool IsInteractable();
    }
}