using UnityEngine;

namespace CommonUtils.WebGL {
	public class WebGLFullScreenKey : MonoBehaviour {
		[SerializeField] private KeyCode key = KeyCode.F;

		private void Update() {
			if (UnityEngine.Input.GetKeyUp(key)) {
				WebGLBridge.GoFullScreen();
			}
		}
	}
}