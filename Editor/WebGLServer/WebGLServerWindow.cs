/*#define COMMONUTILS_WEBSERVER_DEBUG_MODE

using CommonUtils.Editor.SystemProcesses;
using CommonUtils.Verbosables;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.WebGLServer {
	public class WebGLServerWindow : EditorWindow, IVerbosable {
		private enum Status {
			Idle,
			FetchingPythonVersion,
			FetchingStatus,
			Publishing
		}

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

		private readonly CommandLineRunner commandLineRunner = new();
		private string errorMessage;
		private Status currentStatus = Status.Idle;

		public Verbosity Verbosity {
			get {
				#if COMMONUTILS_WEBSERVER_DEBUG_MODE
				return Verbosity.Debug | Verbosity.Warning | Verbosity.Error;
				#else
				return Verbosity.Warning | Verbosity.Error;
				#endif
			}
		}

		private void checkPythonVersion() {
			this.Log("Checking python version");
			errorMessage = null;
			currentStatus = Status.FetchingPythonVersion;
			commandLineRunner.Run("python3",
				"version",
				//ButlerPath,
				onSuccess: s => Debug.Log("Success"),
				onFailed: (code, s) => Debug.Log("Failed"),
				onFinished: () => {
					this.Log("Version command finished.");
					currentStatus = Status.Idle;
				});
		}
	}
}*/