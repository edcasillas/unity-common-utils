using CommonUtils.Verbosables;
using System;
using System.Collections;
using UnityEngine;

namespace CommonUtils.WebGL {
	public partial class WebGLBridge {
		[SerializeField] private float timeToWaitForPointerLockedEvent = 0.05f;

		private event Action<bool> onPointerLockChangedEvent;

		private bool wantedNewPointerIsLockedValue;
		private bool receivedFollowUp = false;

		private bool pointerIsLocked;
		public bool PointerIsLocked {
			get => pointerIsLocked;
			private set {
				if (pointerIsLocked == value) return;
				pointerIsLocked = value;
				this.Log($"Pointer is now {(pointerIsLocked ? "locked" : "unlocked")}");
				onPointerLockChangedEvent?.Invoke(pointerIsLocked);
			}
		}

		public void SubscribeToOnPointerLockChanged(Action<bool> s) => onPointerLockChangedEvent += s;
		public void UnsubscribeFromOnPointerLockChanged(Action<bool> s) => onPointerLockChangedEvent -= s;

		private void setupPointerLockEvents() {
			this.Log("Called setup pointer lock events.");
#if UNITY_WEBGL && !UNITY_EDITOR
			commonUtils_webGL_setupPointerLockEvents(gameObject.name);
#endif
		}

		private void removePointerLockEvents() {
			this.Log("Called remove pointer lock events.");
#if UNITY_WEBGL && !UNITY_EDITOR
			commonUtils_webGL_removePointerLockEvents();
#endif
		}

		// Called from WebGLBridge.jslib
		[ShowInInspector]
		public void OnPointerLockChanged(int locked) {
			this.Log(() => $"OnPointerLockChanged {locked}");
			if (locked > 0) {
				PointerIsLocked = true;
				receivedFollowUp = true;
				return;
			}

			StartCoroutine(handlePointerUnlocked());
		}

		/// <summary>
		/// Waits for a bit to see if there's a second call to OnPointerLockChanged, and does not set it as unlocked if
		/// another call arrived in a short amount of time.
		/// </summary>
		/// <returns></returns>
		private IEnumerator handlePointerUnlocked() {
			receivedFollowUp = false;
			yield return new WaitForSeconds(timeToWaitForPointerLockedEvent);
			if(receivedFollowUp) yield break;
			PointerIsLocked = false;
		}
	}
}
