#define PUBLITCH_DEBUG_MODE

using CommonUtils.Editor.BuiltInIcons;
using CommonUtils.Editor.SystemProcesses;
using CommonUtils.Verbosables;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using File = UnityEngine.Windows.File;

namespace CommonUtils.Editor.Publitch {
	public class PublitchWindow : EditorWindow, IVerbosable {
		#region Constants
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
				instance.initialize();
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
			get => EditorPrefs.GetString(getEditorPrefKey(EDITOR_PREF_KEY_PROJECT_NAME), PlayerSettings.productName);
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

		private readonly SystemProcessRunner fetchVersionProcessRunner;
		private Process fetchVersionProcess;
		private string version;

		private Process fetchStatusProcess;
		private ButlerStatus status = new ButlerStatus();

		private Process publishProcess;
		private string publishResult;

		private static string totalBuildSize;

		private bool isInitialized;

		public Verbosity Verbosity {
			get {
				#if PUBLITCH_DEBUG_MODE
				return Verbosity.Debug | Verbosity.Warning | Verbosity.Error;
				#else
				return Verbosity.Warning | Verbosity.Error;
				#endif
			}
		}

		public PublitchWindow() {
			Debug.Log("Publitch Window constructed");
			fetchVersionProcessRunner = new SystemProcessRunner("butler", "version", onSuccess: parseButlerVersionFromProcessOutput, onFailed: onButlerError );
		}

		private void initialize() {
			if(isInitialized) return;
			this.Log("Publitch is initializing.");
			titleContent = new GUIContent("Publitch");
			maxSize = new Vector2(400f, 300f);
		}

		private void OnEnable() {
			if(EditorApplication.isPlayingOrWillChangePlaymode) return;
			initialize();
			this.Log("PUBLITCH IS EXECUTING");
			checkButlerVersion();
			EditorApplication.update += Update;
			if(!string.IsNullOrEmpty(buildId)) fetchStatusProcess = executeButler($"status {buildId}");
		}

		private void OnDisable() => EditorApplication.update -= Update;

		private void parseButlerVersionFromProcessOutput(string processOutput) {
			version = processOutput;
			if (string.IsNullOrEmpty(version)) return;
			var indexOfComma = version.IndexOf(',');
			if (indexOfComma > 0) {
				version = version.Substring(0, indexOfComma);
			}
		}

		private void onButlerError(Win32ErrorCode errorCode, string errorMessage) {
			this.errorMessage = errorMessage;
		}

		private void Update() {
			if(EditorApplication.isPlayingOrWillChangePlaymode) return;
			/*if (fetchVersionProcess != null) {
				if (fetchVersionProcess.HasExited) {
					this.Log($"Check butler version finished with code {fetchVersionProcess.ExitCode}");
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
			}*/

			if (fetchStatusProcess != null) {
				if (fetchStatusProcess.HasExited) {
					var statusString = fetchStatusProcess.StandardOutput.ReadToEnd();
					if (fetchStatusProcess.ExitCode == 0) {
						ButlerStatus.TryParse(statusString, ref status);
					} else {
						if (statusString.ToLower().Contains("no credentials")) {
							//Debug.LogError("NOT LOGGED IN");
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
			this.Log("Checking butler version");
			/*if (fetchVersionProcess != null) {
				Debug.LogError("Already checking butler version.");
				return;
			}
			fetchVersionProcess = executeButler("version");*/
			fetchVersionProcessRunner.CommandPath = ButlerPath;
			fetchVersionProcessRunner.Start();
		}

		private Process executeButler(string args, DataReceivedEventHandler onOutputDataReceived = null) {
			var processFileName = string.IsNullOrEmpty(ButlerPath) || ButlerPath == NO_FOLDER_SELECTED ? "butler" : Path.Combine(ButlerPath, "butler");
			this.Log($"Executing butler from \"{processFileName}\" with args \"{args}\"");

			var startInfo = new ProcessStartInfo {
				FileName = processFileName,
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
			};
			if (!string.IsNullOrEmpty(ButlerApiKey)) {
				startInfo.Environment.Add("BUTLER_API_KEY", ButlerApiKey);
			}

			var proc = new Process { StartInfo = startInfo };

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
					this.Log($"Butler exited with code {ex.NativeErrorCode} - Not found: {ex.Message}", LogLevel.Error);
				}
				return null;
			}

			return proc;
		}

		private string publishData = string.Empty;
		private float publishProgressPct = 0f;
		private void OnPublishDataReceived(object sender, DataReceivedEventArgs e) {
			if(string.IsNullOrWhiteSpace(e?.Data)) return; // Ignore empty lines.
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
			if(EditorApplication.isPlayingOrWillChangePlaymode) {
				EditorGUILayout.HelpBox("Publitch is not available on Play mode.", MessageType.Info);
				return;
			}

			if (!string.IsNullOrEmpty(errorMessage)) {
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
			}

			if (fetchVersionProcessRunner.IsRunning) {
				this.ShowLoadingSpinner("Checking butler installation...");
			}

			if (string.IsNullOrEmpty(version)) {
				handleNullVersion();
				return;
			}

			EditorGUILayout.HelpBox($"butler {version}", MessageType.None);

			EditorGUILayout.BeginHorizontal();
			var apiKey = EditorGUILayout.TextField("Butler API Key", ButlerApiKey);
			if(apiKey != null) ButlerApiKey.Value = apiKey;
			if (GUILayout.Button(EditorIcon.SearchIcon.ToGUIContent("Go to API Keys on itch"), EditorStyles.iconButton, GUILayout.Height(16))) {
				Application.OpenURL("https://itch.io/user/settings/api-keys");
			}
			EditorGUILayout.EndHorizontal();

			if (string.IsNullOrEmpty(ButlerApiKey)) {
				EditorGUILayout.HelpBox("Add your wharf API key from https://itch.io/user/settings/api-keys.", MessageType.Warning);
				return;
			}

			var user = EditorGUILayout.TextField("User", User);
			if (user != User) User = user;

			var projectName = EditorGUILayout.TextField(new GUIContent("Project Name", "As registered on itch."), ProjectName);
			if (projectName != ProjectName) ProjectName = projectName;

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EnumPopup("Current Build Target", BuildTarget);
			if (GUILayout.Button(EditorIcon.BuildSettingsEditor.ToGUIContent("Open Build Settings"), EditorStyles.iconButton, GUILayout.Height(16))) {
				EditorApplication.ExecuteMenuItem("File/Build Settings...");
			}
			EditorGUILayout.EndHorizontal();

			if (!string.IsNullOrEmpty(BuildPath)) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.TextField("Build Path", BuildPath);
				if (GUILayout.Button(EditorIcon.AnimationVisibilityToggleOn.ToGUIContent("Reveal"), EditorStyles.iconButton, GUILayout.Height(16))) {
					EditorUtility.RevealInFinder(BuildPath);
				}
				EditorGUILayout.EndHorizontal();

				if (totalBuildSize == null) totalBuildSize = getBuildSizeMBString(BuildPath);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Build Size", totalBuildSize ?? "<unknown>");
				if (GUILayout.Button(EditorIcon.TreeEditorRefresh.ToGUIContent("Refresh"), EditorStyles.iconButton, GUILayout.Height(16))) {
					totalBuildSize = getBuildSizeMBString(BuildPath);
				}
				EditorGUILayout.EndHorizontal();
			}

			if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(ProjectName)) {
				EditorGUILayout.HelpBox("To continue please enter your User and Project Name.", MessageType.Warning);
				return;
			}

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextField("Build ID:", buildId);
			if (GUILayout.Button("View on itch.io", EditorStyles.miniButtonRight)) {
				Application.OpenURL($"https://{User}.itch.io/{ProjectName}");
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			if (fetchStatusProcess == null) {
				if (GUILayout.Button("Status")) {
					fetchStatusProcess = executeButler($"status {buildId}");
				}
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
			if (!string.IsNullOrEmpty(LastBuiltDateTime)) EditorGUILayout.LabelField("Last built", LastBuiltDateTime);
			EditorGUILayout.LabelField("Last published", !string.IsNullOrEmpty(LastPublishDateTime) ? LastPublishDateTime : "<unknown>");

			if (publishProcess == null) {
				if (!string.IsNullOrEmpty(BuildPath)) {
					if (GUILayout.Button("Publitch NOW")) {
						publishData = string.Empty;
						publishProcess = executeButler($"push {BuildPath} {buildId}", OnPublishDataReceived);
					}
				} else {
					EditorGUILayout.HelpBox("To continue please build your project.", MessageType.Warning);
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
			totalBuildSize = getBuildSizeMBString(BuildPath);
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

		private static string getBuildSizeMBString(string path) {
			var sizeBytes = getBuildSizeBytes(path);
			var totalSizeMB = sizeBytes / (1024f * 1024f);
			return $"{totalSizeMB:F2} MB";
		}

		private static long getBuildSizeBytes(string path) {
			if (File.Exists(path))
			{
				// It's a file, return its size
				return new FileInfo(path).Length;
			}

			if (Directory.Exists(path))
			{
				// It's a directory, return its total size
				return getDirectorySizeInBytes(new DirectoryInfo(path));
			}

			// Invalid path, handle accordingly
			Debug.LogError("Invalid path: " + path);
			return 0;
		}

		private static long getDirectorySizeInBytes(DirectoryInfo dir) {
			// Add file sizes.
			var files = dir.GetFiles();
			var size = files.Sum(file => file.Length);

			// Add subdirectory sizes.
			var dirs = dir.GetDirectories();
			size += dirs.Sum(getDirectorySizeInBytes);
			return size;
		}
	}
}