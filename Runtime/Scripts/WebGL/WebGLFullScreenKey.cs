using UnityEngine;

public class WebGLFullScreenKey : MonoBehaviour {
	[SerializeField] private KeyCode key = KeyCode.F;

	private void Update() {
		if (Input.GetKeyUp(key)) {
			WebGLBridge.GoFullScreen();
		}
	}
}