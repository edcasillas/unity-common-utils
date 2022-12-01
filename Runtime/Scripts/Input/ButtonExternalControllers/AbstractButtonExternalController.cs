using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using CommonUtils.UnityComponents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CommonUtils.Input.ButtonExternalControllers {
	[RequireComponent(typeof(Selectable))]
	public abstract class AbstractButtonExternalController : MonoBehaviour, IVerbosable, IButtonExternalController {
#pragma warning disable 649
		[SerializeField] private bool isBlockedBySceneLoader = true;
		/// <summary>
		/// Objects that can block this button when they're active.
		/// </summary>
		[Tooltip("Objects that can block this button when they're active.")]
		#if !UNITY_2021_1_OR_NEWER
		[Reorderable]
		#endif
		[SerializeField]  protected List<GameObject> IsBlockedBy;

		[Tooltip("Disable the functionality of this binding when Unity Remote is connected.")]
		[SerializeField] private bool disableWithUnityRemote;

		[SerializeField] private bool verbose;
#pragma warning restore 649

		#region Properties
		private Selectable _button;
		public Selectable Button {
			get {
				if (!_button) {
					_button = GetComponent<Selectable>();
					if (!_button) {
						Debug.LogError($"\"{name}\" doesn't have a required component of type '{nameof(Selectable)}'.");
						enabled = false;
					}
				}
				return _button;
			}
		}

		private readonly HashSet<IButtonExternalControllerBlocker> _currentlyBlockedBy = new HashSet<IButtonExternalControllerBlocker>();

		[ShowInInspector] public IEnumerable<IButtonExternalControllerBlocker> CurrentlyBlockedBy => _currentlyBlockedBy;

		public bool IsVerbose => verbose;
		#endregion

		#region Unity Lifecycle
		protected virtual void Awake() {
			#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isRemoteConnected && disableWithUnityRemote) {
				Destroy(this);
				return;
			}
			#endif

			if (IsBlockedBy.Any(blocker => !blocker)) {
				Debug.LogWarning($"\"{name}\" is being blocked by an invalid object.", this);
			}

			subscribeToBlockers(IsBlockedBy, true);
		}

		private void OnDestroy() => subscribeToBlockers(IsBlockedBy, false);
		#endregion

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
			ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerClickHandler);
		}

		public bool IsInteractable() => !IsBlocked();

		public void AddBlockers(IEnumerable<GameObject> blockers) {
			if(blockers == null) return;
			var changed = false;
			foreach (var blocker in blockers) {
				if (!blocker) {
					Debug.LogWarning($"Trying to add an invalid blocker to \"{name}\".", this);
					continue;
				}

				if (IsBlockedBy.Contains(blocker)) {
					Debug.LogWarning($"Trying to add a duplicated blocker (\"{blocker.name}\") to \"{name}\".", this);
					continue;
				}

				IsBlockedBy.Add(blocker);
				subscribeToBlocker(blocker, true);
				changed = true;
			}
			if(changed) OnBlockersChanged();
		}

		public virtual void OnBlockerBecameActive(IButtonExternalControllerBlocker blocker) => _currentlyBlockedBy.SafeAdd(blocker);

		public virtual void OnBlockerBecameInactive(IButtonExternalControllerBlocker blocker) => _currentlyBlockedBy.SafeRemove(blocker);

		/// <summary>
		/// Gets a value indicating whether any of the blockers are active or not.
		/// </summary>
		/// <returns><c>true</c> when any of the blockers is active, otherwise <c>false</c>.</returns>
		protected virtual bool IsBlocked() {
			if (!gameObject.activeInHierarchy) return true;

			Selectable selectable;
			try { selectable = Button; }
			catch (Exception ex) {
				Debug.LogError($"An error occured while trying to access the {nameof(Selectable)} component of {name}. This {GetType()} object will be deactivated: {ex.Message}", this);
				gameObject.SetActive(false);
				return true;
			}
			if (!selectable.IsInteractable()) return true;

			if (isBlockedBySceneLoader && SceneLoader.IsActive) return true;

			return _currentlyBlockedBy.Any();
		}

		protected virtual void OnBlockersChanged() { }

		private void subscribeToBlockers(IEnumerable<GameObject> blockers, bool subscribe) {
			foreach (var blocker in blockers) {
				subscribeToBlocker(blocker, subscribe);
			}
		}

		private void subscribeToBlocker(GameObject blocker, bool subscribe) {
			var blockerComponent = blocker.GetCachedComponent<ButtonExternalControllerBlocker>();
			if (!blockerComponent.IsValid()) {
				blockerComponent = blocker.AddComponent<ButtonExternalControllerBlocker>();
			}
			if(subscribe) blockerComponent.Subscribe(this);
			else blockerComponent.Unsubscribe(this);
		}
	}
}
