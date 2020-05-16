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

		#region Unity Lifecycle
		private void Awake() => awake.Invoke();

		private void Start() => start.Invoke();

		private void OnEnable() => onEnable.Invoke();

		private void OnDisable() => onDisable.Invoke();

		private void OnDestroy() => onDestroy.Invoke();

		private void OnBecameVisible() => onBecameVisible.Invoke();

		private void OnBecameInvisible() => onBecameInvisible.Invoke();
		#endregion

		#region Public Methods
		public void SubscribeAwake(UnityAction call) => awake.AddListener(call);
		public void UnsubscribeAwake(UnityAction call) => awake.RemoveListener(call);
		public void SubscribeOnEnable(UnityAction call) => onEnable.AddListener(call);
		public void UnsubscribeOnEnable(UnityAction call) => onEnable.RemoveListener(call);
		public void SubscribeStart(UnityAction call) => start.AddListener(call);
		public void UnsubscribeStart(UnityAction call) => start.RemoveListener(call);
		public void SubscribeOnDisable(UnityAction call) => onDisable.AddListener(call);
		public void UnsubscribeOnDisable(UnityAction call) => onDisable.RemoveListener(call);
		public void SubscribeOnDestroy(UnityAction call) => onDestroy.AddListener(call);
		public void UnsubscribeOnDestroy(UnityAction call) => onDestroy.RemoveListener(call);
		public void SubscribeOnBecameVisible(UnityAction call) => onBecameInvisible.AddListener(call);
		public void UnsubscribeOnBecameVisible(UnityAction call) => onBecameInvisible.RemoveListener(call);
		public void SubscribeOnBecameInvisible(UnityAction call) => onBecameInvisible.AddListener(call);
		public void UnsubscribeOnBecameInvisible(UnityAction call) => onBecameInvisible.RemoveListener(call);
		#endregion
	}
}