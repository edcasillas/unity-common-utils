using CommonUtils.UnityComponents;

namespace CommonUtils.Input.SequentialKeyboardNavigation
{
    public interface ISequentialKeyboardNavigationManager : IUnityComponent {
        void Subscribe(ISequentialKeyboardNavigationItem item);
        void Unsubscribe(ISequentialKeyboardNavigationItem item);
        void OnItemEnabledOrDisabled();
    }
}