using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.WebGLServer {
	public class WebGLServerWindow : EditorWindow {
		private static WebGLServerWindow instance;

		[MenuItem("Tools/WebGL Server...")]
		private static void openConfigWindow() => openActiveWindow();

		private static void openActiveWindow() {
			if (!instance) {
				instance = GetWindow<WebGLServerWindow>();
				instance.titleContent = new GUIContent("WebGL Server");
				instance.maxSize = new Vector2(400f, 300f);
			}

			instance.Show();
		}
	}
}