using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers {
	[AddComponentMenu("Input/Button from Mouse")]
	public class ButtonFromMouse : AbstractButtonExternalController {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
		#region Inspector fields
#pragma warning disable 649
		[Tooltip("Mouse Button used to simulate button click.")]
		[SerializeField] private int mouseButton;
#pragma warning restore 649
		#endregion

		#region Properties
		public int MouseButton {
			get => mouseButton;
			set => mouseButton = value;
		}
		#endregion

		private void Update() {
			if (Button.IsInteractable() && !IsBlocked()) {
				if (UnityEngine.Input.GetMouseButtonDown(mouseButton)) {
					this.DebugLog($"{name} was pressed using mouse button {mouseButton}");
					Press();
				}

				if (UnityEngine.Input.GetMouseButtonUp(mouseButton)) {
					this.DebugLog($"{name} was unpressed using key {mouseButton}");
					Release();
				}
			}
		}
#endif
	}
}