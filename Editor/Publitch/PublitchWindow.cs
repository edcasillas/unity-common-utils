using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using File = UnityEngine.Windows.File;

namespace CommonUtils.Editor.Publitch {
	public class EditorPrefsString {
		private readonly string editorPrefsKey;
		private readonly string defaultValue;
		private readonly bool isProjectSpecific;

		private string actualKey;
		private string ActualKey => actualKey ??= $"{(isProjectSpecific ? $"{PlayerSettings.productGUID}." : string.Empty)}{editorPrefsKey}";

		public EditorPrefsString(string editorPrefsKey, string defaultValue = null, bool isProjectSpecific = false) {
			this.editorPrefsKey = editorPrefsKey;
			this.defaultValue = defaultValue;
			this.isProjectSpecific = isProjectSpecific;
		}

		public string Value {
			get => EditorPrefs.GetString(ActualKey, defaultValue);
			set => EditorPrefs.SetString(ActualKey, value);
		}

		public void Clear() => EditorPrefs.DeleteKey(editorPrefsKey);

		public static implicit operator string(EditorPrefsString editorPrefsString) => editorPrefsString.Value;
	}

	public class PublitchWindow : EditorWindow {
		#region Constants
		private const bool DEBUG_MODE = true;
		private const string EDITOR_PREF_BUTLER_FOLDER_PATH = "ButlerFolderPath";
		private const string EDITOR_PREF_BUTLER_API_KEY = "ButlerApiKey";
		private const string EDITOR_PREF_KEY_PREFIX = "Publitch";
		private const string EDITOR_PREF_KEY_BUILD_TARGET = "LastKnownBuildTarget";
		private const string EDITOR_PREF_KEY_BUILD_PATH = "LastKnownBuildPath";
		private const string EDITOR_PREF_KEY_USER = "User";
		private const string EDITOR_PREF_KEY_PROJECT_NAME = "ProjectName";
		private const string EDITOR_PREF_KEY_LAST_PUBLISH_DATETIME = "LastPublishDateTime";
		private const string EDITOR_PREF_KEY_LAST_BUILD_DATETIME = "LastBuiltDateTime";
		private const string NO_FOLDER_SELECTED = "<no folder selected>";
		#endregion

		#region Statics (To create the editor menu and save preferences)
		private static PublitchWindow instance;

		[MenuItem("Tools/Publish to itch.io...")]
		private static void openConfigWindow() => openActiveWindow();

		private static void openActiveWindow() {
			if (!instance) {
				instance = GetWindow<PublitchWindow>();
				instance.titleContent = new GUIContent("Publitch");
				instance.maxSize = new Vector2(400f, 300f);
			}

			instance.Show();
		}

		#region Properties (connected to EditorPrefs)
		internal static readonly EditorPrefsString ButlerPath = new EditorPrefsString(EDITOR_PREF_BUTLER_FOLDER_PATH, string.Empty);
		internal static readonly EditorPrefsString ButlerApiKey = new EditorPrefsString(getEditorPrefKey(EDITOR_PREF_BUTLER_API_KEY), string.Empty, true);

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

		internal static string LastPublishDateTime {
			get => EditorPrefs.GetString(getEditorPrefKey(EDITOR_PREF_KEY_LAST_PUBLISH_DATETIME));
			set => EditorPrefs.SetString(getEditorPrefKey(EDITOR_PREF_KEY_LAST_PUBLISH_DATETIME), value);
		}
		internal static string LastBuiltDateTime {
			get => EditorPrefs.GetString(getEditorPrefKey(EDITOR_PREF_KEY_LAST_BUILD_DATETIME));
			set => EditorPrefs.SetString(getEditorPrefKey(EDITOR_PREF_KEY_LAST_BUILD_DATETIME), value);
		}

		private static string buildId => $"{User}/{ProjectName}:{getChannelName(BuildTarget)}";
		#endregion
		#endregion

		private string errorMessage;

		private Process fetchVersionProcess;
		private string version;

		private Process fetchStatusProcess;
		private ButlerStatus status = new ButlerStatus();

		private Process publishProcess;
		private string publishResult;

		private void OnEnable() {
			checkButlerVersion();
			EditorApplication.update += Update;
			if(!string.IsNullOrEmpty(buildId)) fetchStatusProcess = executeButler($"status {buildId}");
		}

		private void OnDisable() => EditorApplication.update -= Update;

		private void Update() {
			if (fetchVersionProcess != null) {
				if (fetchVersionProcess.HasExited) {
					debugLog($"Check butler version finished with code {fetchVersionProcess.ExitCode}");
					if (fetchVersionProcess.ExitCode == 0) {
						version = fetchVersionProcess.StandardOutput.ReadToEnd();
						if (!string.IsNullOrEmpty(version)) {
							var indexOfComma = version.IndexOf(',');
							if (indexOfComma > 0) {
								version = version.Substring(0, indexOfComma);
								//version = version[..indexOfComma];
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
					var statusString = fetchStatusProcess.StandardOutput.ReadToEnd();
					if (fetchStatusProcess.ExitCode == 0) {
						ButlerStatus.TryParse(statusString, ref status);
					} else {
						if (statusString.ToLower().Contains("no credentials")) {
							Debug.LogError("NOT LOGGED IN");
						} else {
							errorMessage = "An error occurred while trying to fetch the status of the project.";
						}
					}

					fetchStatusProcess = null;
				}
			}

			if (publishProcess != null) {
				if (publishProcess.HasExited) {
					if (publishProcess.ExitCode == 0) {
						errorMessage = null;
						//publishResult = publishProcess.StandardOutput.ReadToEnd();
						fetchStatusProcess = executeButler($"status {buildId}");
						LastPublishDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);

						//EditorUtility.DisplayDialog("Publitch", "Your project has been publ-ITCH-ed!", "Sweet!");
					} else {
						errorMessage = "An error occurred while publishing.";
					}

					publishProcess = null;
				}
			}
		}

		private void checkButlerVersion() {
			debugLog("Checking butler version");
			if (fetchVersionProcess != null) {
				Debug.LogError("Already checking butler version.");
				return;
			}
			fetchVersionProcess = executeButler("version");
		}

		private Process executeButler(string args, DataReceivedEventHandler onOutputDataReceived = null) {
			var processFileName = string.IsNullOrEmpty(ButlerPath) || ButlerPath == NO_FOLDER_SELECTED ? "butler" : Path.Combine(ButlerPath, "butler");
			debugLog($"Executing butler from \"{processFileName}\" with args \"{args}\"");
			var proc = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = processFileName,
					Arguments = args,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					//WorkingDirectory = @"C:\MyAndroidApp\"
				}
			};

			if (onOutputDataReceived != null) proc.OutputDataReceived += onOutputDataReceived;

			try {
				proc.Start();
				if (onOutputDataReceived != null) proc.BeginOutputReadLine();
			} catch (Win32Exception ex) {
				// Error 2 is not found; if this is the error, we'll show an option to install
				// butler in the editor window. Otherwise it's an unknown error and we'll log it.
				if (ex.NativeErrorCode != 2) {
					Debug.LogException(ex);
				} else {
					debugLog($"Butler exited with code {ex.NativeErrorCode} - Not found: {ex.Message}");
				}
				return null;
			}

			return proc;
		}

		private string publishData = string.Empty;
		private float publishProgressPct = 0f;
		private void OnPublishDataReceived(object sender, DataReceivedEventArgs e) {
			if (ButlerParser.TryParseProgress(e?.Data, out var pct)) {
				publishProgressPct = pct;
			} else {
				Debug.LogWarning($"Didn't find percentage in string: '{e?.Data}'");
			}

			publishData = e?.Data;
		}

		private void handleNullVersion() { // TODO Choose a better name for this method!
			var errorMsg = "Couldn't find butler in the " + (string.IsNullOrEmpty(ButlerPath) ? "current environment" : "selected install folder") + ".\nMake sure butler is installed properly and select the install folder below. \nTo install butler, please visit https://itchio.itch.io/butler";
			EditorGUILayout.HelpBox(errorMsg, MessageType.Error);

			if (GUILayout.Button("Go to itch.io/butler")) {
				Application.OpenURL("https://itchio.itch.io/butler");
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(string.IsNullOrEmpty(ButlerPath) ? NO_FOLDER_SELECTED : ButlerPath);
			if (GUILayout.Button("Select")) {
				var folder = EditorUtility.OpenFolderPanel("Select folder", ButlerPath, "");
				if (!string.IsNullOrEmpty(folder)) {
					if (!File.Exists(Path.Combine(folder, "butler"))) {
						EditorUtility.DisplayDialog("Publitch", "butler not found in selected folder", "OK");
					} else {
						ButlerPath.Value = folder;
						checkButlerVersion();
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Refresh")) {
				checkButlerVersion();
			}
		}

		private void OnGUI() {
			if (!string.IsNullOrEmpty(errorMessage)) {
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
			}

			if (fetchVersionProcess != null) {
				// TODO Replace HelpBox with EditorExtensions.ShowLoadingSpinner once main gets merged in.
				EditorGUILayout.HelpBox("Checking butler installation", MessageType.Info);
				return;
			}

			if (string.IsNullOrEmpty(version)) {
				handleNullVersion();
				return;
			}

			EditorGUILayout.HelpBox($"butler {version}", MessageType.None);

			EditorGUILayout.BeginHorizontal();
			var apiKey = EditorGUILayout.TextField("Butler API Key", ButlerApiKey);
			if(apiKey != null) ButlerApiKey.Value = apiKey;
			if (GUILayout.Button(EditorIcon.Search.ToGUIContent("Go to API Keys on itch.io"), GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight))) {
				Application.OpenURL("https://itch.io/user/settings/api-keys");
			}
			EditorGUILayout.EndHorizontal();

			if (string.IsNullOrEmpty(ButlerApiKey)) {
				EditorGUILayout.HelpBox("Add your wharf API key from https://itch.io/user/settings/api-keys.", MessageType.Warning);
				return;
			}

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

			if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(ProjectName)) return;

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
				EditorGUILayout.HelpBox($"Fetching status for {timeRunning.TotalSeconds} seconds",
					MessageType.Info);
				Repaint();
			} else {
				if (status.HasData) {
					EditorGUILayout.TextField("Channel", status.ChannelName);
					EditorGUILayout.TextField("Upload", status.Upload);
					EditorGUILayout.TextField("Build", status.Build);
					EditorGUILayout.TextField("Version", status.Version);
				}
			}

			EditorGUILayout.Space();
			if (!string.IsNullOrEmpty(LastBuiltDateTime))
				EditorGUILayout.LabelField("Last built", LastBuiltDateTime);
			if (!string.IsNullOrEmpty(LastPublishDateTime))
				EditorGUILayout.LabelField("Last published", LastPublishDateTime);
			if (publishProcess == null) {
				if (GUILayout.Button("Publitch NOW")) {
					publishData = string.Empty;
					publishProcess = executeButler($"push {BuildPath} {buildId}", OnPublishDataReceived);
				}
			} else {
				var timeRunning = DateTime.Now - publishProcess.StartTime;
				EditorGUILayout.HelpBox($"Publishing to itch for {timeRunning.TotalSeconds} seconds",
					MessageType.Info);
				if (GUILayout.Button("Cancel")) {
					if (EditorUtility.DisplayDialog("Cancel publish", "Are you sure?", "Yup", "Nope")) {
						publishProcess.Kill();
						publishProcess = null;
					}
				}

				Repaint();
			}

			if (!string.IsNullOrEmpty(publishData)) {
				var progressBarRect = EditorGUILayout.GetControlRect();
				EditorGUI.ProgressBar(progressBarRect, publishProgressPct / 100f, $"{publishProgressPct}%");
				EditorGUILayout.TextArea(publishData);
			}
		}

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
			LastBuiltDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
			BuildTarget = target; // TODO is this really needed
			BuildPath = pathToBuiltProject;
		}

		private static string getEditorPrefKey(string setting) => $"{EDITOR_PREF_KEY_PREFIX}.{setting}";

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

		private static void debugLog(string message) {
			if(DEBUG_MODE) Debug.Log(message);
		}
	}
}