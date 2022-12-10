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

		private string errorMessage;

		private Process fetchVersionProcess;
		private string version;

		private Process fetchStatusProcess;
		private string status;

		private Process publishProcess;
		private string publishResult;

		private static void openActiveWindow() {
			if (!instance) {
				instance = GetWindow<PublitchWindow>();
				instance.titleContent = new GUIContent("Publitch");
				instance.maxSize = new Vector2(400f, 300f);
			}

			instance.Show();
		}

		private void OnEnable() {
			fetchVersionProcess = checkButlerVersion();
			EditorApplication.update += Update;
		}

		private void OnDisable() => EditorApplication.update -= Update;

		private void Update() {
			if (fetchVersionProcess != null) {
				if (fetchVersionProcess.HasExited) {
					if (fetchVersionProcess.ExitCode == 0) {
						version = fetchVersionProcess.StandardOutput.ReadToEnd();
						if (!string.IsNullOrEmpty(version)) {
							var indexOfComma = version.IndexOf(',');
							if (indexOfComma > 0) {
								version = version[..indexOfComma];
							}
						}
					} else {
						errorMessage = "An error occurred while trying to fetch the version of butler.";
					}

					fetchVersionProcess = null;
				}
			}

			if (fetchStatusProcess != null) {
				if (fetchStatusProcess.HasExited) {
					if (fetchStatusProcess.ExitCode == 0) {
						status = fetchStatusProcess.StandardOutput.ReadToEnd();
					} else {
						errorMessage = "An error occurred while trying to fetch the status of the project.";
					}

					fetchStatusProcess = null;
				}
			}

			if (publishProcess != null) {
				if (publishProcess.HasExited) {
					if (publishProcess.ExitCode == 0) {
						publishResult = publishProcess.StandardOutput.ReadToEnd();
						fetchStatusProcess = executeButler($"status {buildId}");
						EditorUtility.DisplayDialog("Publitch", "Your project has been publ-ITCH-ed!", "Sweet!");
					} else {
						errorMessage = "An error occurred while publishing.";
					}

					publishProcess = null;
				}
			}
		}

		private Process checkButlerVersion() => executeButler("version");

		private Process executeButler(string args) {
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

			return proc;
		}

		private void OnGUI() {
			if (!string.IsNullOrEmpty(errorMessage)) {
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
			}

			if (fetchVersionProcess != null) {
				EditorGUILayout.HelpBox($"Checking butler installation", MessageType.Info);
				return;
			}

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

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.TextField("Build Path", BuildPath);
				if (GUILayout.Button("Reveal", EditorStyles.miniButtonRight)) {
					EditorUtility.RevealInFinder(BuildPath);
				}
				EditorGUILayout.EndHorizontal();

				if(string.IsNullOrEmpty(User) || string.IsNullOrEmpty(ProjectName)) return;

				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.TextField("Build ID:", buildId);
				if (GUILayout.Button("View on itch.io", EditorStyles.miniButtonRight)) {
					Application.OpenURL($"https://{User}.itch.io/{ProjectName}");
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();
				if (GUILayout.Button("Status")) {
					fetchStatusProcess = executeButler($"status {buildId}");
				}

				if (fetchStatusProcess != null) {
					var timeRunning = DateTime.Now - fetchStatusProcess.StartTime;
					EditorGUILayout.HelpBox($"Fetching status for {timeRunning.TotalSeconds} seconds" , MessageType.Info);
					Repaint();
				} else {
					if (!string.IsNullOrEmpty(status)) {
						EditorGUILayout.TextArea(status);
					}
				}

				EditorGUILayout.Space();
				if (GUILayout.Button("Publitch NOW")) {
					publishProcess = executeButler($"push {BuildPath} {buildId}");
				}

				if (publishProcess != null) {
					var timeRunning = DateTime.Now - publishProcess.StartTime;
					EditorGUILayout.HelpBox($"Publishing to itch for {timeRunning.TotalSeconds} seconds" , MessageType.Info);
					Repaint();
				}
			}
			else EditorGUILayout.HelpBox("Butler is not installed", MessageType.Error); // TODO Give instructions on how to install
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