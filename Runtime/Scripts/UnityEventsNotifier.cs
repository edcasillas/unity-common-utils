using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils {
	/// <summary>
	/// A component that allows other GameObjects be notified when a Unity Event happens to this one.
	/// </summary>
	public class UnityEventsNotifier : MonoBehaviour {
#pragma warning disable 649
		[SerializeField] private UnityEvent awake;
		[SerializeField] private UnityEvent onEnable;
		[SerializeField] private UnityEvent start;
		[SerializeField] private UnityEvent onBecameVisible;
		[SerializeField] private UnityEvent onBecameInvisible;
		[SerializeField] private UnityEvent onDisable;
		[SerializeField] private UnityEvent onDestroy;
#pragma warning restore 649

		private void Awake() => awake.Invoke();

		private void Start() => start.Invoke();

		private void OnEnable() => onEnable.Invoke();

		private void OnDisable() => onDisable.Invoke();

		private void OnDestroy() => onDestroy.Invoke();

		private void OnBecameVisible() => onBecameVisible.Invoke();

		private void OnBecameInvisible() => onBecameInvisible.Invoke();
	}
}