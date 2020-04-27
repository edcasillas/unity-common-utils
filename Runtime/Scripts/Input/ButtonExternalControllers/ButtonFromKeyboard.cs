using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers {
	/// <summary>
	/// Allows simulating button clicks using a key from the keyboard.
	/// </summary>
	[AddComponentMenu("Input/Button from Keyboard")]
	public class ButtonFromKeyboard : AbstractButtonExternalController {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
		#region Inspector fields
#pragma warning disable 649
		[Tooltip("Key used to simulate button click.")]
		[SerializeField] private KeyCode keyCode;
#pragma warning restore 649
		#endregion

		#region Properties
		public KeyCode KeyCode {
			get => keyCode;
			set => keyCode = value;
		}
		#endregion

		private void Update() {
			if (Button.IsInteractable() && !IsBlocked()) {
				if (UnityEngine.Input.GetKeyDown(keyCode)) {
					this.DebugLog($"{name} was pressed using key {keyCode}");
					Press();
				}

				if (UnityEngine.Input.GetKeyUp(keyCode)) {
					this.DebugLog($"{name} was unpressed using key {keyCode}");
					Release();
				}
			}
		}
	}
#endif
}