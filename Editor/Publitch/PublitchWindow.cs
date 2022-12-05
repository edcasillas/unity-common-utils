using System.ComponentModel;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CommonUtils.Editor.Publitch {
	public class PublitchWindow : EditorWindow {
		private static PublitchWindow instance;

		[MenuItem("Tools/Publish to itch.io...")]
		private static void openConfigWindow() => openActiveWindow();

		private string version;
		private BuildTarget currentBuildTarget;
		private string buildPath;

		private static void openActiveWindow() {
			if (!instance) {
				instance = GetWindow<PublitchWindow>();
				instance.titleContent = new GUIContent("Publitch");
				instance.maxSize = new Vector2(400f, 300f);
			}

			instance.Show();
		}

		private void OnEnable() {
			version = checkButlerVersion();
			if (!string.IsNullOrEmpty(version)) {
				var indexOfComma = version.IndexOf(',');
				if (indexOfComma > 0) {
					version = version[..indexOfComma];
				}
			}

			if (string.IsNullOrEmpty(version)) return;

			currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
		}

		private string checkButlerVersion() {
			var proc = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = @"butler",
					Arguments = "version",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					//WorkingDirectory = @"C:\MyAndroidApp\"
				}
			};

			try {
				proc.Start();
			} catch (Win32Exception ex) {
				// Error 2 is not found; if this is the error, we'll show an option to install
				// butler in the editor window. Otherwise it's an unknown error and we'll log it.
				if (ex.NativeErrorCode != 2) {
					Debug.LogException(ex);
				}
				return null;
			}
			proc.WaitForExit();
			return proc.StandardOutput.ReadToEnd();
		}

		private void OnGUI() {
			if (!string.IsNullOrEmpty(version)) {
				EditorGUILayout.HelpBox($"butler {version}", MessageType.None);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.EnumPopup("Current Build Target", currentBuildTarget);
				if (GUILayout.Button("Change...", EditorStyles.miniButtonRight)) {
					EditorApplication.ExecuteMenuItem("File/Build Settings...");
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.TextField("Build Path", buildPath);
			}
			else EditorGUILayout.HelpBox("Butler is not installed", MessageType.Error);
		}

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
			instance.currentBuildTarget = target;
			instance.buildPath = pathToBuiltProject;
		}
	}
}