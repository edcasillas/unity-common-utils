using CommonUtils.UnityComponents;
using UnityEngine.Events;

namespace CommonUtils.UI.Submenus {
	public interface ISubmenu : IUnityComponent {
		bool IsInitialized { get; }

		/// <summary>
		/// Gets a value indicating whether this submenu is being shown.
		/// </summary>
		/// <remarks>Formerly called 'IsOpen'.</remarks>
		bool IsShown { get; }

		void Show();
		void Hide();

		void SubscribeOnShown(UnityAction action);
		void SubscribeOnHidden(UnityAction action);
		void UnsubscribeOnShown(UnityAction action);
		void UnsubscribeOnHidden(UnityAction action);
	}
}