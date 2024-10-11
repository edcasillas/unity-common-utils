using CommonUtils.Editor.BuiltInIcons;
using CommonUtils.Editor.EditorPrefsValues;
using CommonUtils.Editor.SystemProcesses;
using CommonUtils.Extensions;
using CommonUtils.Logging;
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
			FetchingButlerVersion,
			FetchingPythonVersion,
			FetchingStatus,
			RunningWebServer,
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
		private const string EDITOR_PREF_KEY_WEBGL_COMPRESSION_METHOD = "WebGLCompressionMethod";
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
		internal static readonly EditorPrefsString ButlerApiKey = new(getEditorPrefKey(EDITOR_PREF_BUTLER_API_KEY), string.Empty);
		internal static readonly EditorPrefsString BuildPath = new(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_PATH), string.Empty, true);
		internal static readonly EditorPrefsString User = new(getEditorPrefKey(EDITOR_PREF_KEY_USER), string.Empty, true);
		internal static readonly EditorPrefsString ProjectName = new(getEditorPrefKey(EDITOR_PREF_KEY_PROJECT_NAME), () => PlayerSettings.productName, true);
		internal static readonly EditorPrefsString LastPublishDateTime = new(getEditorPrefKey(EDITOR_PREF_KEY_LAST_PUBLISH_DATETIME), string.Empty, true);
		internal static readonly EditorPrefsString LastBuiltDateTime = new(getEditorPrefKey(EDITOR_PREF_KEY_LAST_BUILD_DATETIME), string.Empty, true);
		internal static readonly EditorPrefsEnum<BuildTarget> LastBuildTargetPlatform = new(getEditorPrefKey(EDITOR_PREF_KEY_BUILD_TARGET), () => EditorUserBuildSettings.activeBuildTarget, true);
		internal static readonly EditorPrefsEnum<WebGLCompressionFormat> BuildCompressionFormat = new(getEditorPrefKey(EDITOR_PREF_KEY_WEBGL_COMPRESSION_METHOD), (WebGLCompressionFormat)(-1), true);
		#endregion
		private static string buildId => $"{User}/{ProjectName}:{getChannelName(LastBuildTargetPlatform)}";

		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
			LastBuiltDateTime.Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
			LastBuildTargetPlatform.Value = target;
			BuildPath.Value = pathToBuiltProject;
			if(target == BuildTarget.WebGL) BuildCompressionFormat.Value = PlayerSettings.WebGL.compressionFormat;
			totalBuildSize = getBuildSizeMBString(BuildPath);
		}
		#endregion

		private Status currentStatus = Status.Idle;
		private string errorMessage;

		private readonly CommandLineRunner commandLineRunner = new();

		private string butlerVersion;
		private string pythonVersion;
		private string webServerPort = "8080";
		private ButlerStatus projectStatus;
		private static string totalBuildSize;

		private bool isInitialized;

		private bool showSettings = false;
		private bool showAdvancedSettings = false;
		private bool showWebServerSettings = false;

		private string fetchStatusErrorMessage;

		public Verbosity Verbosity { get; private set; } = Verbosity.Warning | Verbosity.Error;

		private string webServerUrl => $"http://{NetworkAddress.CurrentIpV4}:{webServerPort}";

		private bool IsWebGLBuildCompressed => LastBuildTargetPlatform == BuildTarget.WebGL &&
											   (BuildCompressionFormat == WebGLCompressionFormat.Gzip ||
												BuildCompressionFormat == WebGLCompressionFormat.Brotli);

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
			if (commandLineRunner.IsRunning) {
				this.Log("Killing current command line runner.");
				commandLineRunner.Kill();
			}
		}

		private void checkButlerVersion(bool showErrorDialog = false) {
			this.Log("Checking butler version");
			errorMessage = null;
			currentStatus = commandLineRunner.Run("butler",
				"version",
				ButlerPath,
				onSuccess: processOutput => butlerVersion = processOutput.Split(',').FirstOrDefault(),
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
					if(!string.IsNullOrEmpty(butlerVersion)) fetchStatus();
				}) ? Status.FetchingButlerVersion : Status.Idle;
		}

		private void runWebServer() {
			this.Log("Running web server");
			errorMessage = null;
			currentStatus = commandLineRunner.Run("python3",
				$"-m http.server {webServerPort} --directory {BuildPath.Value}",
				onSuccess: processOutput => this.Log("Web server finished successfully."),
				onFailed: (errorCode, errorMsg) => {
					errorMessage = errorMsg;
				},
				onFinished: () => {
					this.Log("Web server finished.");
					currentStatus = Status.Idle;
				}) ? Status.RunningWebServer : Status.Idle;
			if(currentStatus == Status.RunningWebServer) Application.OpenURL(webServerUrl);
		}

		private void checkPythonVersion(bool showErrorDialog = false) {
			this.Log("Checking python version");
			errorMessage = null;

			currentStatus = commandLineRunner.Run("python3",
				"--version",
				//ButlerPath,
				onSuccess: processOutput => pythonVersion = processOutput.Trim(),
				onFailed: (errorCode, errorMsg) => {
					if (errorCode == Win32ErrorCode.ERROR_FILE_NOT_FOUND) {
						errorMessage = "Couldn't find python in the " + (string.IsNullOrEmpty("") ? "current environment" : "selected install folder") + ".\nMake sure butler is installed properly and select the install folder.";
						if(showErrorDialog) EditorUtility.DisplayDialog("Error", errorMessage, "OK");
						return;
					}
					errorMessage = errorMsg;
				},
				onFinished: () => {
					this.Log("Python version command finished.");
					currentStatus = Status.Idle;
				}) ? Status.FetchingPythonVersion : Status.Idle;
		}

		private void fetchStatus() {
			if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(ProjectName) || string.IsNullOrEmpty(buildId)) return;
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
			fetchStatusErrorMessage = ButlerStatus.TryParse(processOutput, ref projectStatus) ? null : processOutput;
		}

		private void onButlerStatusError(Win32ErrorCode errorCode, string errorMessage) => this.errorMessage = errorMessage;

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

			if (!string.IsNullOrEmpty(errorMessage)) { EditorGUILayout.HelpBox(errorMessage, MessageType.Error); }

			if (currentStatus == Status.FetchingButlerVersion) { this.ShowLoadingSpinner("Checking butler installation..."); }

			showSettings |= string.IsNullOrEmpty(butlerVersion);
			renderBulterSettings();

			if (string.IsNullOrEmpty(butlerVersion)) { return; }

			if (string.IsNullOrEmpty(ButlerApiKey)) {
				//EditorGUILayout.HelpBox("Add your wharf API key from https://itch.io/user/settings/api-keys.", MessageType.Warning);
				return;
			}

			drawUserInput();
			drawProjectNameInput();
			EditorGUILayout.Space();
			drawBuildTarget();

			if (!string.IsNullOrEmpty(BuildPath)) {
				EditorExtensions.FolderField("Build Path", BuildPath, isReadOnly: true);

				if (totalBuildSize == null) totalBuildSize = getBuildSizeMBString(BuildPath);
				drawLabelWithRightButton("Build Size", totalBuildSize ?? "<unknown>", EditorIcon.TreeEditorRefresh, () => totalBuildSize = getBuildSizeMBString(BuildPath), "Refresh");

				if (LastBuildTargetPlatform == BuildTarget.WebGL) {
					var compressionFormatString = (BuildCompressionFormat == (WebGLCompressionFormat)(-1) ? "<unknown>" : BuildCompressionFormat.ToString());
					drawLabelWithRightButton("WebGL Compression", compressionFormatString, EditorIcon.SettingsIcon, () => EditorApplication.ExecuteMenuItem("Edit/Project Settings..."));
				}
			}

			if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(ProjectName)) {
				EditorGUILayout.HelpBox("To continue please enter your User and Project Name.", MessageType.Warning);
				return;
			}

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorExtensions.ReadOnlyLabelField("Build ID", buildId);
			if (GUILayout.Button("View on itch.io", EditorStyles.miniButtonRight)) {
				Application.OpenURL($"https://{User}.itch.io/{ProjectName}");
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			if (LastBuildTargetPlatform == BuildTarget.WebGL) {
				showWebServerSettings = EditorExtensions.Collapse(showWebServerSettings,
					"Run web server",
					() => {
						if (BuildCompressionFormat == WebGLCompressionFormat.Gzip ||
							BuildCompressionFormat == WebGLCompressionFormat.Brotli) {
							EditorGUILayout.HelpBox("Publitch's web server cannot serve compressed files. " +
													"Change the compression format to Disabled in the Player Settings to use this functionality. " +
													"You can try to run the web server anyway.",
								MessageType.Warning);
						}

						if (currentStatus == Status.FetchingButlerVersion) {
							this.ShowLoadingSpinner("Fetching Python3 version");
						} else {
							renderVersionField("Python Version", pythonVersion, () => checkPythonVersion(true));
						}

						webServerPort = EditorGUILayout.TextField("Port", webServerPort);

						EditorGUILayout.BeginHorizontal();
						EditorExtensions.ReadOnlyLabelField("Local URL", webServerUrl);
						if (EditorIcon.Clipboard.Button()) {
							webServerUrl.CopyToClipboard();
						}

						if (currentStatus == Status.RunningWebServer && EditorIcon.PlayButton.Button()) {
							Application.OpenURL(webServerUrl);
						}

						EditorGUILayout.EndHorizontal();

						if (currentStatus == Status.Idle && GUILayout.Button("Run web server")) {
							runWebServer();
						}

						if (currentStatus == Status.RunningWebServer) {
							this.ShowLoadingSpinner("Running web server");
							if (GUILayout.Button("Stop web server")) {
								commandLineRunner.Kill();
								currentStatus = Status.Idle;
							}
						}
					});
				EditorGUILayout.Space();
			}

			EditorGUILayout.Space();
			if (!commandLineRunner.IsRunning) {
				if (GUILayout.Button("Status")) {
					fetchStatus();
				}
			}

			if (currentStatus == Status.FetchingStatus) {
				this.ShowLoadingSpinner($"Fetching status for {commandLineRunner.ExecutionTime?.TotalSeconds ?? 0} seconds");
			} else {
				if (!string.IsNullOrEmpty(fetchStatusErrorMessage)) {
					EditorGUILayout.HelpBox(fetchStatusErrorMessage, MessageType.Error);
				}
				if (projectStatus.HasData) {
					EditorGUILayout.TextField("Channel", projectStatus.ChannelName);
					EditorGUILayout.TextField("Upload", projectStatus.Upload);
					EditorGUILayout.TextField("Build", projectStatus.Build);
					EditorGUILayout.TextField("Version", projectStatus.Version);
				}
			}

			EditorGUILayout.Space();
			if (!string.IsNullOrEmpty(LastBuiltDateTime)) EditorGUILayout.LabelField("Last built", LastBuiltDateTime);

			// TODO The following line some times logs this error: ArgumentException: Getting control 20's position in a group with only 20 controls when doing repaint Aborting
			EditorGUILayout.LabelField("Last published", !string.IsNullOrEmpty(LastPublishDateTime) ? LastPublishDateTime : "<unknown>");

			if (currentStatus == Status.Idle) {
				if (!string.IsNullOrEmpty(BuildPath)) {
					if (LastBuildTargetPlatform == BuildTarget.WebGL) {
						if (!IsWebGLBuildCompressed) {
							EditorGUILayout.HelpBox("Your WebGL build is not compressed. You can change the compression format on Player Settings.", MessageType.Warning);
						}
					}

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

		private void renderVersionField(string label, string value, Action refreshDelegate) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(label, !string.IsNullOrEmpty(value) ? value : "<not found>");
			if (EditorIcon.TreeEditorRefresh.Button("Refresh")) {
				refreshDelegate?.Invoke();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void renderBulterSettings() => showSettings = EditorExtensions.Collapse(showSettings,
			$"butler {(!string.IsNullOrEmpty(butlerVersion)? butlerVersion: "(Setup Required)")}",
			() => {
				EditorExtensions.FolderField("Butler Path", ButlerPath,
					newButlerPath => {
						if (!File.Exists(Path.Combine(newButlerPath, "butler"))) {
							EditorUtility.DisplayDialog("Publitch", "butler not found in selected folder", "OK");
						} else {
							ButlerPath.Value = newButlerPath;
							checkButlerVersion(true);
						}
					});

				renderVersionField("Butler Version", butlerVersion, () => checkButlerVersion(true));
				drawAPIKeyInput();

				showAdvancedSettings = EditorExtensions.Collapse(showAdvancedSettings,
					"Advanced Settings",
					() => {
						if (GUILayout.Button("Go to itch.io/butler")) {
							Application.OpenURL("https://itchio.itch.io/butler");
						}
						Verbosity = (Verbosity)EditorGUILayout.EnumFlagsField("Publitch Verbosity", Verbosity);
						commandLineRunner.Verbosity = Verbosity;
						if (EditorExtensions.Button("Clear settings", backgroundColor: Color.red, fontStyle: FontStyle.Italic)) {
							if (EditorUtility.DisplayDialog("Publitch Settings", "Are you sure you want to delete all settings?", "Yes", "Cancel")) {
								clearButlerSettings();
							}
						}
					});
			},
			false,
			true);

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
			EditorGUILayout.EnumPopup("Current Build Target", LastBuildTargetPlatform);
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
			butlerVersion = null;
		}

		private void drawLabelWithRightButton(string label, string text, EditorIcon buttonIcon, Action onButtonPressed, string tooltip = null) {
			EditorGUILayout.BeginHorizontal();
			EditorExtensions.ReadOnlyLabelField(label, text);
			if (buttonIcon.Button(tooltip)) {
				onButtonPressed?.Invoke();
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}