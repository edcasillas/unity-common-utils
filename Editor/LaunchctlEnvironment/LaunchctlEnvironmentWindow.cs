using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CommonUtils.Editor.LaunchctlEnvironment {
	public class LaunchctlEnvironmentWindow : EditorWindow {
		private static LaunchctlEnvironmentWindow instance;

		private readonly List<LaunchctlEnvironmentVariable> variables = new();
		private Vector2 scroll;
		private string search = string.Empty;
		private string error;
		private string status;

		[MenuItem("Tools/Common Utils/Launchctl Environment Variables")]
		private static void open() {
			if (!instance) {
				instance = GetWindow<LaunchctlEnvironmentWindow>();
				instance.titleContent = new GUIContent("launchctl Env");
				instance.minSize = new Vector2(650f, 350f);
			}

			instance.Show();
			instance.refreshVariables();
		}

		private void OnEnable() => refreshVariables();

		private void OnGUI() {
			if (Application.platform != RuntimePlatform.OSXEditor) {
				EditorGUILayout.HelpBox("launchctl environment variables are only available in the macOS editor.", MessageType.Warning);
				return;
			}

			drawToolbar();

			if (!string.IsNullOrEmpty(error)) {
				EditorGUILayout.HelpBox(error, MessageType.Error);
			} else if (!string.IsNullOrEmpty(status)) {
				EditorGUILayout.HelpBox(status, MessageType.Info);
			}

			drawHeader();
			drawVariables();
		}

		private void drawToolbar() {
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			EditorGUILayout.LabelField("Search", GUILayout.Width(45f));
			search = EditorGUILayout.TextField(search, EditorStyles.toolbarTextField, GUILayout.MinWidth(160f));
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70f))) {
				refreshVariables();
				GUI.FocusControl(null);
			}

			EditorGUILayout.EndHorizontal();
		}

		private void drawHeader() {
			EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(220f));
			EditorGUILayout.LabelField("Value", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Status", EditorStyles.boldLabel, GUILayout.Width(130f));
			EditorGUILayout.EndHorizontal();
		}

		private void drawVariables() {
			var visibleVariables = GetVisibleVariables().ToList();
			if (visibleVariables.Count == 0) {
				EditorGUILayout.HelpBox("No launchctl environment variables match the current search.", MessageType.Info);
				return;
			}

			scroll = EditorGUILayout.BeginScrollView(scroll);
			foreach (var variable in visibleVariables) {
				drawVariable(variable);
			}
			EditorGUILayout.EndScrollView();
		}

		private IEnumerable<LaunchctlEnvironmentVariable> GetVisibleVariables() {
			if (string.IsNullOrWhiteSpace(search)) return variables;

			return variables.Where(variable => variable.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
		}

		private void drawVariable(LaunchctlEnvironmentVariable variable) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.SelectableLabel(variable.Name, GUILayout.Width(220f), GUILayout.Height(EditorGUIUtility.singleLineHeight));

			EditorGUI.BeginChangeCheck();
			var newValue = EditorGUILayout.DelayedTextField(variable.Value);
			if (EditorGUI.EndChangeCheck()) {
				saveVariable(variable, newValue);
			}

			EditorGUILayout.LabelField(variable.Status, GUILayout.Width(130f));
			EditorGUILayout.EndHorizontal();
		}

		private void refreshVariables() {
			if (Application.platform != RuntimePlatform.OSXEditor) return;

			error = null;
			status = null;
			var uidResult = runLaunchctl("manageruid");
			if (!uidResult.Success) {
				error = $"Could not get launchctl manager uid: {uidResult.Error}";
				return;
			}

			var uid = uidResult.Output.Trim();
			var printResult = runLaunchctl("print", $"gui/{uid}");
			if (!printResult.Success) {
				error = $"Could not read launchctl environment: {printResult.Error}";
				return;
			}

			variables.Clear();
			variables.AddRange(LaunchctlEnvironmentParser.ParseEnvironmentVariables(printResult.Output));
			status = $"Loaded {variables.Count} launchctl environment variables from gui/{uid}.";
			Repaint();
		}

		private void saveVariable(LaunchctlEnvironmentVariable variable, string newValue) {
			var result = runLaunchctl("setenv", variable.Name, newValue);
			if (!result.Success) {
				variable.Status = "Save failed";
				error = $"Could not save {variable.Name}: {result.Error}";
				Debug.LogError(error);
				return;
			}

			variable.Value = newValue;
			variable.Status = "Saved";
			error = null;
			status = $"Saved {variable.Name}. New processes launched from this launchctl domain will receive the updated value.";
		}

		private static LaunchctlResult runLaunchctl(params string[] arguments) {
			var process = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = "/bin/launchctl",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};

			process.StartInfo.Arguments = string.Join(" ", arguments.Select(quoteArgument));

			process.Start();
			var output = process.StandardOutput.ReadToEnd();
			var stderr = process.StandardError.ReadToEnd();
			process.WaitForExit();

			return new LaunchctlResult(process.ExitCode == 0, output, stderr.Trim());
		}

		private static string quoteArgument(string argument) {
			if (string.IsNullOrEmpty(argument)) return "\"\"";

			return $"\"{argument.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
		}

		private readonly struct LaunchctlResult {
			public bool Success { get; }
			public string Output { get; }
			public string Error { get; }

			public LaunchctlResult(bool success, string output, string error) {
				Success = success;
				Output = output;
				Error = string.IsNullOrEmpty(error) ? output.Trim() : error;
			}
		}
	}
}
