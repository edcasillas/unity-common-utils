using System;
using UnityEngine;

namespace CommonUtils
{
    public class DebuggableProxy : MonoBehaviour {
		[SerializeField] private MonoBehaviour target;

		public event Action OnTargetChanged;

		private MonoBehaviour previousTarget;

		public MonoBehaviour Target => target;

		private void Awake() => previousTarget = target;

		private void Update() {
			if (target != previousTarget) {
				previousTarget = target;
				OnTargetChanged?.Invoke();
			}
		}
	}
}
