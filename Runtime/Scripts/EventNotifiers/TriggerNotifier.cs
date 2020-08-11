using UnityEngine;

namespace CommonUtils.EventNotifiers {
	[RequireComponent(typeof(Collider))]
	public class TriggerNotifier : MonoBehaviour {
		#pragma warning disable 649
		[SerializeField] private ColliderEvent onTriggerEnter;
		[SerializeField] private ColliderEvent onTriggerExit;
		#pragma warning restore 649

		private void OnTriggerEnter(Collider other) => onTriggerEnter?.Invoke(other);

		private void OnTriggerExit(Collider other) => onTriggerExit?.Invoke(other);
	}
}
