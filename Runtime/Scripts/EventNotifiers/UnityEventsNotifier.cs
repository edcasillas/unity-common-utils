using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils.EventNotifiers {
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
		private void Awake() => awake?.Invoke();

		private void Start() => start?.Invoke();

		private void OnEnable() => onEnable?.Invoke();

		private void OnDisable() => onDisable?.Invoke();

		private void OnDestroy() {
			onDestroy?.Invoke();
			UnsubscribeAll();
		}

		private void OnBecameVisible() => onBecameVisible?.Invoke();

		private void OnBecameInvisible() => onBecameInvisible?.Invoke();
		#endregion

		#region Public Methods
		public void SubscribeAwake(UnityAction call) {
			if (awake == null) awake = new UnityEvent();
			awake.AddListener(call);
		}
		public void UnsubscribeAwake(UnityAction call) => awake?.RemoveListener(call);

		public void SubscribeOnEnable(UnityAction call) {
			if (onEnable == null) onEnable = new UnityEvent();
			onEnable.AddListener(call);
		}
		public void UnsubscribeOnEnable(UnityAction call) => onEnable?.RemoveListener(call);

		public void SubscribeStart(UnityAction call) {
			if(start == null) start = new UnityEvent();
			start.AddListener(call);
		}
		public void UnsubscribeStart(UnityAction call) => start?.RemoveListener(call);

		public void SubscribeOnDisable(UnityAction call) {
			if(onDisable == null) onDisable = new UnityEvent();
			onDisable.AddListener(call);
		}
		public void UnsubscribeOnDisable(UnityAction call) => onDisable?.RemoveListener(call);

		public void SubscribeOnDestroy(UnityAction call) {
			if (onDestroy == null) onDestroy = new UnityEvent();
			onDestroy.AddListener(call);
		}
		public void UnsubscribeOnDestroy(UnityAction call) => onDestroy?.RemoveListener(call);

		public void SubscribeOnBecameVisible(UnityAction call) {
			if (onBecameVisible == null) onBecameVisible = new UnityEvent();
			onBecameVisible.AddListener(call);
		}
		public void UnsubscribeOnBecameVisible(UnityAction call) => onBecameInvisible?.RemoveListener(call);

		public void SubscribeOnBecameInvisible(UnityAction call) {
			if(onBecameInvisible == null) onBecameInvisible = new UnityEvent();
			onBecameInvisible.AddListener(call);
		}
		public void UnsubscribeOnBecameInvisible(UnityAction call) => onBecameInvisible?.RemoveListener(call);

		public void UnsubscribeAll() {
			awake?.RemoveAllListeners();
			onEnable?.RemoveAllListeners();
			start?.RemoveAllListeners();
			onDisable?.RemoveAllListeners();
			onDestroy?.RemoveAllListeners();
			onBecameVisible?.RemoveAllListeners();
			onBecameInvisible?.RemoveAllListeners();
		}
		#endregion
	}
}