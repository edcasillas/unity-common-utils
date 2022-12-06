using System;
using System.ComponentModel;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CommonUtils.Editor.Publitch {
	public class PublitchWindow : EditorWindow {
		private const string EDITOR_PREF_KEY_PREFIX = "Publitch";
		private const string EDITOR_PREF_KEY_BUILD_TARGET = "LastKnownBuildTarget";
		private const string EDITOR_PREF_KEY_BUILD_PATH = "LastKnownBuildPath";
		private const string EDITOR_PREF_KEY_USER = "User";
		private const string EDITOR_PREF_KEY_PROJECT_NAME = "ProjectName";

		private static PublitchWindow instance;

		[MenuItem("Tools/Publish to itch.io...")]
		private static void openConfigWindow() => openActiveWindow();

		#region Properties (connected to EditorPrefs)
		internal static BuildTarget BuildTarget {
			get => (BuildTarget)EditorPrefs.GetInt(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_TARGET), (int)EditorUserBuildSettings.activeBuildTarget);
			set => EditorPrefs.SetInt(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_TARGET), (int)value);
		}

		internal static string BuildPath {
			get => EditorPrefs.GetString(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_PATH));
			set => EditorPrefs.SetString(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_PATH), value);
		}

		internal static string User {
			get => EditorPrefs.GetString(getEditorPrefKey(EDITOR_PREF_KEY_USER));
			set => EditorPrefs.SetString(getEditorPrefKey(EDITOR_PREF_KEY_USER), value);
		}

		internal static string ProjectName {
			get => EditorPrefs.GetString(getEditorPrefKey(EDITOR_PREF_KEY_PROJECT_NAME));
			set => EditorPrefs.SetString(getEditorPrefKey(EDITOR_PREF_KEY_PROJECT_NAME), value);
		}

		private static string buildId => $"{User}/{ProjectName}:{getChannelName(BuildTarget)}";
		#endregion

		private string version;
		private string status;

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
		}

		private string checkButlerVersion() => executeButler("version");

		private string executeButler(string args) {
			var proc = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = @"butler",
					Arguments = args,
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
			Debug.Log($"{args} - returned {proc.ExitCode}");
			return proc.StandardOutput.ReadToEnd();
		}

		private void OnGUI() {
			if (!string.IsNullOrEmpty(version)) {
				EditorGUILayout.HelpBox($"butler {version}", MessageType.None);

				var user = EditorGUILayout.TextField("User", User);
				if (user != User) User = user;

				var projectName = EditorGUILayout.TextField("Project Name", ProjectName);
				if (projectName != ProjectName) ProjectName = projectName;

				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.EnumPopup("Current Build Target", BuildTarget);
				if (GUILayout.Button("Change...", EditorStyles.miniButtonRight)) {
					EditorApplication.ExecuteMenuItem("File/Build Settings...");
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.TextField("Build Path", BuildPath);

				if(string.IsNullOrEmpty(User) || string.IsNullOrEmpty(ProjectName)) return;

				EditorGUILayout.Space();
				EditorGUILayout.TextField("Build ID:", buildId);

				EditorGUILayout.Space();
				if (GUILayout.Button("Status")) {
					status = executeButler($"status {buildId}");
				}

				if (!string.IsNullOrEmpty(status)) {
					EditorGUILayout.TextArea(status);
				}
			}
			else EditorGUILayout.HelpBox("Butler is not installed", MessageType.Error);
		}

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
			BuildTarget = target; // TODO is this really needed
			BuildPath = pathToBuiltProject;
		}

		private static string getEditorPrefKey(string setting) {
			return $"{PlayerSettings.productGUID}.{EDITOR_PREF_KEY_PREFIX}.{setting}";
		}

		private static string getChannelName(BuildTarget t) {
			switch (t) {
				case BuildTarget.StandaloneOSX:
					return "mac";
				case BuildTarget.StandaloneWindows64:
					return "windows-x64";
				case BuildTarget.StandaloneWindows:
					return "windows";
				case BuildTarget.Android:
					return "android";
				case BuildTarget.WebGL:
					return "html";
				default:
					return ""; // Everything else is not supported
			}

		}
	}
}