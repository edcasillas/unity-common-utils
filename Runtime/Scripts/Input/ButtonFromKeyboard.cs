using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using CommonUtils.Inspector.ReorderableInspector;
using SubjectNerd.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CommonUtils.Input {
	/// <summary>
	/// Allows simulating button clicks using a key from the keyboard.
	/// </summary>
	[RequireComponent(typeof(Button))]
	[AddComponentMenu("Input/Button from Keyboard")]
	public class ButtonFromKeyboard : MonoBehaviour, IVerbosable {
		#region Inspector fields
		#pragma warning disable 649
		/// <summary>
		/// Objects that can block this button when they're active.
		/// </summary>
		[Tooltip("Objects that can block this button when they're active.")]
		[SerializeField] [Reorderable] private List<GameObject> isBlockedBy;

		[Tooltip("Key used to simulate button click.")]
		[SerializeField] private KeyCode keyCode;

		[SerializeField]
		private bool verbose;
		#pragma warning restore 649
		#endregion

		#region Properties
		private Button _button;

		private Button button {
			get {
				if (!_button) {
					_button = GetComponent<Button>();
					if (!_button) {
						Debug.LogError($"\"{name}\" doesn't have a required component of type '{nameof(Button)}'.");
						enabled = false;
					}
				}
				return _button;
			}
		}

		public bool IsVerbose => verbose;
		#endregion

		private void Awake() {
			if (isBlockedBy.Any(blocker => blocker == null)) {
				Debug.LogWarning($"\"{name}\" is being blocked by an invalid object.");
			}
		}

		private void Update() {
			if (button.IsInteractable() && !isBlocked()) {
				var pointer = new PointerEventData(EventSystem.current); // pointer event for Execute

				if (UnityEngine.Input.GetKeyDown(keyCode)) {
					ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
					ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
					button.OnSelect(pointer);
				}

				if (UnityEngine.Input.GetKeyUp(keyCode) && EventSystem.current.currentSelectedGameObject == gameObject) {
					ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
					button.onClick.Invoke();
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether any of the blockers are active or not.
		/// </summary>
		/// <returns><c>true</c> when any of the blockers is active, otherwise <c>false</c>.</returns>
		private bool isBlocked() {
			if (isBlockedBy.IsNullOrEmpty()) return false;
			var activeBlocker = isBlockedBy.FirstOrDefault(blocker => blocker.activeInHierarchy);
			if (activeBlocker) {
				this.DebugLog($"Button from keyboard \"{name}\" [key = {keyCode}] is being blocked by {activeBlocker.name}");
				return true;
			}
			return false;
		}
	}
}