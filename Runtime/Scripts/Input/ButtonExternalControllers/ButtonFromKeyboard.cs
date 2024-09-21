using CommonUtils.Extensions;
using CommonUtils.Verbosables;
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
			if (IsInteractable()) {
				if (IsKeyPressed()) {
					this.Log($"{name} was pressed using key {keyCode}");
					Press();
				}

				if (IsKeyReleased()) {
					this.Log($"{name} was unpressed using key {keyCode}");
					Release();
				}
			}
		}

		protected virtual bool IsKeyPressed() => UnityEngine.Input.GetKeyDown(keyCode);
		protected virtual bool IsKeyReleased() => UnityEngine.Input.GetKeyUp(keyCode);
#endif
	}
}