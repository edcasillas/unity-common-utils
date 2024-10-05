using UnityEngine;

namespace CommonUtils {
	public class SimpleRotator : MonoBehaviour {
		[SerializeField] private Vector3 speed;

		private void Update() => transform.Rotate(speed * Time.deltaTime);
	}
}
