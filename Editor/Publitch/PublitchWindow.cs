// #define PUBLITCH_DEBUG_MODE

using CommonUtils.Editor.BuiltInIcons;
using CommonUtils.Editor.SystemProcesses;
using CommonUtils.Verbosables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CommonUtils.Editor.Publitch {
	public class PublitchWindow : EditorWindow, IVerbosable {
		private enum Status {
			Idle,
			FetchingVersion,
			FetchingStatus,
			Publishing
		}

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
		internal static readonly EditorPrefsString ButlerPath = new(EDITOR_PREF_BUTLER_FOLDER_PATH, string.Empty);
		internal static readonly EditorPrefsString ButlerApiKey = new(getEditorPrefKey(EDITOR_PREF_BUTLER_API_KEY), string.Empty, true);
		internal static readonly EditorPrefsString BuildPath = new(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_PATH), string.Empty, true);
		internal static readonly EditorPrefsString User = new(getEditorPrefKey(EDITOR_PREF_KEY_USER), string.Empty, true);
		internal static readonly EditorPrefsString ProjectName = new(getEditorPrefKey(EDITOR_PREF_KEY_PROJECT_NAME), () => PlayerSettings.productName, true);
		internal static readonly EditorPrefsString LastPublishDateTime = new(getEditorPrefKey(EDITOR_PREF_KEY_LAST_PUBLISH_DATETIME), string.Empty, true);
		internal static readonly EditorPrefsString LastBuiltDateTime = new(getEditorPrefKey(EDITOR_PREF_KEY_LAST_BUILD_DATETIME), string.Empty, true);

		internal static BuildTarget BuildTarget {
			get => (BuildTarget)EditorPrefs.GetInt(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_TARGET), (int)EditorUserBuildSettings.activeBuildTarget);
			set => EditorPrefs.SetInt(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_TARGET), (int)value);
		}

		private static string buildId => $"{User}/{ProjectName}:{getChannelName(BuildTarget)}";
		#endregion
		#endregion

		private Status currentStatus = Status.Idle;
		private string errorMessage;

		private readonly CommandLineRunner commandLineRunner = new(); //{ Verbosity = Verbosity.Debug | Verbosity.Warning | Verbosity.Error };

		private string version;
		private ButlerStatus projectStatus;
		private static string totalBuildSize;

		private bool isInitialized;

		private bool showSettings = false;

		public Verbosity Verbosity {
			get {
				#if PUBLITCH_DEBUG_MODE
				return Verbosity.Debug | Verbosity.Warning | Verbosity.Error;
				#else
				return Verbosity.Warning | Verbosity.Error;
				#endif
			}
		}

		private void initialize() {
			if(isInitialized) return;
			this.Log("Publitch is initializing.");
			titleContent = new GUIContent("Publitch");
			maxSize = new Vector2(400f, 300f);
			isInitialized = true;
		}

		private void OnEnable() {
			if(EditorApplication.isPlayingOrWillChangePlaymode) return;
			this.Log("PUBLITCH IS EXECUTING");
			initialize();
			checkButlerVersion();
		}

		private void OnDisable() {
			if(commandLineRunner.IsRunning) commandLineRunner.Kill();
		}

		private void checkButlerVersion(bool showErrorDialog = false) {
			this.Log("Checking butler version");
			errorMessage = null;
			currentStatus = commandLineRunner.Run("butler",
				"version",
				ButlerPath,
				onSuccess: processOutput => version = processOutput.Split(',').FirstOrDefault(),
				onFailed: (errorCode, errorMsg) => {
					if (errorCode == Win32ErrorCode.ERROR_FILE_NOT_FOUND) {
						errorMessage = "Couldn't find butler in the " + (string.IsNullOrEmpty(ButlerPath) ? "current environment" : "selected install folder") + ".\nMake sure butler is installed properly and select the install folder. \nTo install butler, please visit https://itchio.itch.io/butler";
						if(showErrorDialog) EditorUtility.DisplayDialog("Error", errorMessage, "OK");
						return;
					}
					errorMessage = errorMsg;
				},
				onFinished: () => {
					this.Log("Version command finished.");
					currentStatus = Status.Idle;
					if(!string.IsNullOrEmpty(version)) fetchStatus();
				}) ? Status.FetchingStatus : Status.Idle;
		}

		private void fetchStatus() {
			if (string.IsNullOrEmpty(buildId)) return;
			errorMessage = null;
			projectStatus.Clear();
			currentStatus = Status.FetchingStatus;
			commandLineRunner.Run("butler",
				$"status {buildId}",
				ButlerPath,
				onSuccess: parseButlerStatusFromProcessOutput,
				onFailed: onButlerStatusError,
				onFinished: () => {
					this.Log("Status command finished.");
					currentStatus = Status.Idle;
				},
				environmentVariables: new Dictionary<string, string> { { "BUTLER_API_KEY", ButlerApiKey } });
		}

		private void publish() {
			if (string.IsNullOrEmpty(buildId)) return;
			errorMessage = null;
			publishData = string.Empty;
			currentStatus = Status.Publishing;
			commandLineRunner.Run("butler",
				$"push {BuildPath} {buildId}",
				ButlerPath,
				onOutputDataReceived: OnPublishDataReceived,
				onSuccess: _ => LastPublishDateTime.Value = DateTime.Now.ToString(CultureInfo.InvariantCulture),
				onFailed: onButlerPublishError,
				onFinished: () => {
					currentStatus = Status.Idle;
					publishData = string.Empty;
					publishProgressPct = 0f;
					fetchStatus();
				},
				environmentVariables: new Dictionary<string, string> { { "BUTLER_API_KEY", ButlerApiKey } });
		}

		private void parseButlerStatusFromProcessOutput(string processOutput) {
			this.Log($"Status command success: {processOutput}");
			ButlerStatus.TryParse(processOutput, ref projectStatus);
		}

		private void onButlerStatusError(Win32ErrorCode errorCode, string errorMessage) {
			this.Log(errorMessage, LogLevel.Error);
			this.errorMessage = errorMessage;
		}

		private void onButlerPublishError(Win32ErrorCode errorCode, string errorMessage) {
			this.Log(errorMessage, LogLevel.Error);
			this.errorMessage = errorMessage;
		}

		private string publishData = string.Empty;
		private float publishProgressPct = 0f;
		private void OnPublishDataReceived(string data) {
			var parseResult = ButlerParser.TryParseProgress(data, out var pct);
			switch (parseResult) {
				case ButlerParser.ParseResult.Ok:
					publishProgressPct = pct;
					break;
				case ButlerParser.ParseResult.NotFound:
				case ButlerParser.ParseResult.MultiplePeriodsInNumber:
				case ButlerParser.ParseResult.UnexpectedCharacter:
				case ButlerParser.ParseResult.UnexpectedError:
					Debug.LogWarning($"Didn't find percentage in string: '{data}'");
					break;
				//case ButlerParser.ParseResult.EmptyString:
				//case ButlerParser.ParseResult.ExpectedNonProgress:
				default:
					throw new ArgumentOutOfRangeException();
			}

			publishData = data;
		}

		private void OnGUI() {
			if(EditorApplication.isPlayingOrWillChangePlaymode) {
				EditorGUILayout.HelpBox("Publitch is not available in Play mode.", MessageType.Info);
				return;
			}

			if (!string.IsNullOrEmpty(errorMessage)) {
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
			}

			if (currentStatus == Status.FetchingVersion) {
				this.ShowLoadingSpinner("Checking butler installation...");
			}

			showSettings |= string.IsNullOrEmpty(version);
			showSettings = renderBulterSettings();

			if (string.IsNullOrEmpty(version)) {
				return;
			}

			if (string.IsNullOrEmpty(ButlerApiKey)) {
				EditorGUILayout.HelpBox("Add your wharf API key from https://itch.io/user/settings/api-keys.", MessageType.Warning);
				return;
			}

			drawUserInput();
			drawProjectNameInput();

			EditorGUILayout.Space();
			drawBuildTarget();

			if (!string.IsNullOrEmpty(BuildPath)) {
				EditorExtensions.FolderField("Build Path", BuildPath);

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
			if (!commandLineRunner.IsRunning) {
				if (GUILayout.Button("Status")) {
					fetchStatus();
				}
			}

			if (currentStatus == Status.FetchingStatus) {
				this.ShowLoadingSpinner($"Fetching status for {commandLineRunner.ExecutionTime?.TotalSeconds ?? 0} seconds");
			} else {
				if (projectStatus.HasData) {
					EditorGUILayout.TextField("Channel", projectStatus.ChannelName);
					EditorGUILayout.TextField("Upload", projectStatus.Upload);
					EditorGUILayout.TextField("Build", projectStatus.Build);
					EditorGUILayout.TextField("Version", projectStatus.Version);
				}
			}

			EditorGUILayout.Space();
			if (!string.IsNullOrEmpty(LastBuiltDateTime)) EditorGUILayout.LabelField("Last built", LastBuiltDateTime);
			EditorGUILayout.LabelField("Last published", !string.IsNullOrEmpty(LastPublishDateTime) ? LastPublishDateTime : "<unknown>");

			if (currentStatus == Status.Idle) {
				if (!string.IsNullOrEmpty(BuildPath)) {
					if (EditorExtensions.Button("Publitch NOW", fontColor: Color.white, backgroundColor: Color.green, fontStyle: FontStyle.Bold)) {
						publish();
					}
				} else {
					EditorGUILayout.HelpBox("To continue please build your project.", MessageType.Warning);
				}
			}
			if (currentStatus == Status.Publishing) {
				this.ShowLoadingSpinner("Publishing...", publishProgressPct / 100f); // TODO Label must be "Publishing for XX seconds"
				EditorGUILayout.TextArea(publishData ?? string.Empty);

				if (GUILayout.Button("Cancel")) {
					if (EditorUtility.DisplayDialog("Cancel publish", "Are you sure?", "Yup", "Nope")) {
						commandLineRunner.Kill();
					}
				}
			}
		}

		private bool renderBulterSettings() => EditorExtensions.Collapse(showSettings,
			$"butler {(!string.IsNullOrEmpty(version)? version: "(Setup Required)")}",
			() => {
				EditorExtensions.FolderField("Butler Path", ButlerPath, true,
					newButlerPath => {
						if (!File.Exists(Path.Combine(newButlerPath, "butler"))) {
							EditorUtility.DisplayDialog("Publitch", "butler not found in selected folder", "OK");
						} else {
							ButlerPath.Value = newButlerPath;
							checkButlerVersion(true);
						}
					});

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Butler Version", !string.IsNullOrEmpty(version) ? version : "<not found>");
				if (EditorIcon.TreeEditorRefresh.Button("Refresh")) {
					checkButlerVersion(true);
				}
				EditorGUILayout.EndHorizontal();
				drawAPIKeyInput();

				if (GUILayout.Button("Go to itch.io/butler")) {
					Application.OpenURL("https://itchio.itch.io/butler");
				}

				if (EditorExtensions.Button("Clear settings", backgroundColor: Color.red, fontStyle: FontStyle.Italic)) {
					if (EditorUtility.DisplayDialog("Publitch Settings", "Are you sure you want to delete all settings?", "Yes", "Cancel")) {
						clearButlerSettings();
					}
				}
			},
			false,
			true);

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
			LastBuiltDateTime.Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
			BuildTarget = target; // TODO is this really needed
			BuildPath.Value = pathToBuiltProject;
			totalBuildSize = getBuildSizeMBString(BuildPath);
		}

		private static string getEditorPrefKey(string setting) => $"{EDITOR_PREF_KEY_PREFIX}.{setting}";

		private static void drawAPIKeyInput() {
			EditorGUILayout.BeginHorizontal();
			var apiKey = EditorGUILayout.PasswordField("Butler API Key", ButlerApiKey);
			if (apiKey != null) ButlerApiKey.Value = apiKey;
			if (EditorIcon.SearchIcon.Button("Go to API Keys on itch")) {
				Application.OpenURL("https://itch.io/user/settings/api-keys");
			}

			EditorGUILayout.EndHorizontal();
		}

		private static void drawUserInput() {
			var user = EditorGUILayout.TextField("User", User);
			if (user != User) User.Value = user;
		}

		private static void drawProjectNameInput() {
			var projectName = EditorGUILayout.TextField(new GUIContent("Project Name", "As registered on itch."), ProjectName);
			if (projectName != ProjectName) ProjectName.Value = projectName;
		}

		private static void drawBuildTarget() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.EnumPopup("Current Build Target", BuildTarget);
			if (GUILayout.Button(EditorIcon.BuildSettingsEditor.ToGUIContent("Open Build Settings"), EditorStyles.iconButton, GUILayout.Height(16))) {
				EditorApplication.ExecuteMenuItem("File/Build Settings...");
			}
			EditorGUILayout.EndHorizontal();
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

		private void clearButlerSettings() {
			ButlerPath.Clear();
			ButlerApiKey.Clear();
			version = null;
		}
	}
}