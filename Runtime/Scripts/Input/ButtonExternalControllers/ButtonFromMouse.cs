using System;
using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.Input.ButtonExternalControllers {
	[Obsolete("Please replace usages of this component with " + nameof(ButtonFromKeyboard), true)]
	public class ButtonFromMouse : AbstractButtonExternalController {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
		#region Inspector fields
#pragma warning disable 649
		[Tooltip("Mouse Button used to simulate button click.")]
		[SerializeField] private int mouseButton;
#pragma warning restore 649
		#endregion

		private void Update() {
			if (!IsBlocked()) {
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