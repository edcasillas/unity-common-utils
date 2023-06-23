using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Text;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace CommonUtils.Editor.Android {
	public class APKInstallerWindow : EditorWindow {
		private const string menuPath = "Tools/Android/Install APK...";

		private string selectedDevice = string.Empty;
		private string apkPath = string.Empty;
		private bool isInstalling = false;
		private bool installationSuccess = false;

		[MenuItem(menuPath)]
		private static void openWindow() {
			var window = GetWindow<APKInstallerWindow>();
			window.titleContent = new GUIContent("Install APK");
			window.minSize = new Vector2(400f, 200f);
			window.Show();
		}

		private void OnGUI() {
			EditorGUILayout.Space();

			if (!AndroidEditorUtils.IsADBInstalled()) {
				EditorGUILayout.HelpBox(
					"ADB is not installed. Please make sure Android SDK is correctly installed and configured.",
					MessageType.Error);
				return;
			}

			var connectedDevices = AndroidEditorUtils.GetConnectedDevices();
			selectedDevice = AndroidEditorUtils.DeviceSelector(selectedDevice, connectedDevices);
			drawAPKField();
			drawInstallButton();
			drawCancelButton();
		}
		
		private void drawAPKField() {
			EditorGUILayout.LabelField("APK", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();

			apkPath = EditorGUILayout.TextField(apkPath);

			if (GUILayout.Button("Browse", GUILayout.Width(80))) {
				apkPath = EditorUtility.OpenFilePanel("Select APK", "", "apk");
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
		}

		private void drawInstallButton() {
			EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(selectedDevice) || string.IsNullOrEmpty(apkPath));

			EditorGUILayout.Space();

			if (isInstalling) {
				EditorGUILayout.LabelField("Installing APK...", EditorStyles.boldLabel);
			} else {
				if (GUILayout.Button("Install", GUILayout.Height(30))) {
					installAPKAsync();
				}
			}

			EditorGUI.EndDisabledGroup();
		}

		private void drawCancelButton() {
			if(!isInstalling) return;

			EditorGUILayout.Space();

			if (GUILayout.Button("Cancel", GUILayout.Height(30))) {
				// Cancel installation
				isInstalling = false;
				installationSuccess = false;

				// Kill the ADB process if it's still running
				killADBProcess();
			}
		}

		private async void installAPKAsync() {
			isInstalling = true;
			installationSuccess = false;

			var command = $"-s {selectedDevice} install -r \"{apkPath}\"";
			var startInfo = new ProcessStartInfo {
				FileName = AndroidEditorUtils.ADBPath,
				Arguments = command,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};

			var installOutput = new StringBuilder();
			installOutput.AppendLine($"adb {command}");

			try {
				await Task.Run(() => {
					using (var process = new Process()) {
						process.StartInfo = startInfo;
						process.OutputDataReceived += (_, e) => {
							installOutput.AppendLine(e.Data);
						};

						process.Start();
						process.BeginOutputReadLine();

						process.WaitForExit();

						// Check the exit code to determine if the installation was successful
						installationSuccess = process.ExitCode == 0;
					}
				});
			} catch (Exception exception) {
				isInstalling = false;
				showInstallationResult(false, $"An error occurred: {exception.Message}");
			} finally {
				isInstalling = false;
				Debug.Log(installOutput.ToString());
				showInstallationResult(installationSuccess, null);
			}
		}

		private void showInstallationResult(bool success, string message) {
			var resultMessage = success ? "APK installation succeeded." : "APK installation failed.";
			if (!string.IsNullOrEmpty(message)) {
				resultMessage += Environment.NewLine + message;
			}

			EditorUtility.DisplayDialog("APK Installation", resultMessage, "OK");
		}

		private void killADBProcess() {
			var startInfo = new ProcessStartInfo {
				FileName = AndroidEditorUtils.ADBPath,
				Arguments = "kill-server",
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};

			var process = new Process();
			process.StartInfo = startInfo;
			process.Start();
			process.WaitForExit();
		}
	}
}