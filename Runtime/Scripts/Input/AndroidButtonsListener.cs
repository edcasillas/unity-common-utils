using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils.Input {
	[AddComponentMenu("UI/Android Buttons Listener")]
	public class AndroidButtonsListener : MonoBehaviour {
		#pragma warning disable 649
		[SerializeField] private UnityEvent onBackButtonPressed;
		[SerializeField] private UnityEvent onMenuButtonPressed;
		#pragma warning restore 649

		#if !UNITY_ANDROID
		private void Awake() => Destroy(this.gameObject);
		#endif

		private void Update() {
			if(UnityEngine.Input.GetKey(KeyCode.Escape)) {
				onBackButtonPressed.Invoke();
			}

			if(UnityEngine.Input.GetKey(KeyCode.Menu)) {
				onMenuButtonPressed.Invoke();
			}
		}
	}
}