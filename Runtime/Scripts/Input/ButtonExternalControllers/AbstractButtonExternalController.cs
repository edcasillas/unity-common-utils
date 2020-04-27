using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using CommonUtils.Inspector.ReorderableInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CommonUtils.Input.ButtonExternalControllers {
	[RequireComponent(typeof(Button))]
	public abstract class AbstractButtonExternalController : MonoBehaviour, IVerbosable {
#pragma warning disable 649
		/// <summary>
		/// Objects that can block this button when they're active.
		/// </summary>
		[Tooltip("Objects that can block this button when they're active.")]
		[SerializeField] [Reorderable] protected List<GameObject> IsBlockedBy;

		[SerializeField] private bool verbose;
#pragma warning restore 649

		#region Properties
		private Button button;

		protected Button Button {
			get {
				if (!button) {
					button = GetComponent<Button>();
					if (!button) {
						Debug.LogError($"\"{name}\" doesn't have a required component of type '{nameof(UnityEngine.UI.Button)}'.");
						enabled = false;
					}
				}
				return button;
			}
		}

		public bool IsVerbose => verbose;
		#endregion

		private void Awake() {
			if (IsBlockedBy.Any(blocker => !blocker)) {
				Debug.LogWarning($"\"{name}\" is being blocked by an invalid object.");
			}
		}

		/// <summary>
		/// Simulates pressing this button.
		/// </summary>
		public void Press() {
			var pointer = new PointerEventData(EventSystem.current); // pointer event for Execute
			ExecuteEvents.Execute(Button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
			ExecuteEvents.Execute(Button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
		}

		/// <summary>
		/// Simulates releasing this button (after pressing it).
		/// </summary>
		public void Release() {
			var pointer = new PointerEventData(EventSystem.current); // pointer event for Execute
			ExecuteEvents.Execute(Button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
			Button.onClick.Invoke();
		}

		/// <summary>
		/// Gets a value indicating whether any of the blockers are active or not.
		/// </summary>
		/// <returns><c>true</c> when any of the blockers is active, otherwise <c>false</c>.</returns>
		protected bool IsBlocked() {
			if (IsBlockedBy.IsNullOrEmpty()) return false;
			var activeBlocker = IsBlockedBy.FirstOrDefault(blocker => blocker.activeInHierarchy);
			if (activeBlocker) {
				this.DebugLog($"Button \"{name}\" is being blocked by {activeBlocker.name}");
				return true;
			}
			return false;
		}
	}
}
