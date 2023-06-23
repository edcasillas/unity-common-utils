using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using UnityEditor.Android;
using Debug = UnityEngine.Debug;

namespace CommonUtils.Editor.Android
{
    public class APKInstallerWindow : EditorWindow
    {
        private const string menuPath = "Tools/Android/Install APK...";

        private static string sdkRootPath => AndroidExternalToolsSettings.sdkRootPath;
        private static string adbPath => Path.Combine(sdkRootPath, "platform-tools", GetADBExecutableName());

        private string selectedDevice = string.Empty;
        private string apkPath = string.Empty;
        private bool isInstalling = false;
        private bool installationSuccess = false;
        private float installationProgress = 0f;

        private float progressSliderHeight = 20f;

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
			DrawAPKField();
			DrawInstallButton();
			DrawCancelButton();
		}

		private static string GetADBExecutableName()
        {
            return Application.platform switch
            {
                RuntimePlatform.WindowsEditor => "adb.exe",
                RuntimePlatform.OSXEditor => "adb",
                RuntimePlatform.LinuxEditor => "adb",
                _ => throw new NotSupportedException("Unsupported platform."),
            };
        }

        private void DrawAPKField()
        {
            EditorGUILayout.LabelField("APK", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            apkPath = EditorGUILayout.TextField(apkPath);

            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                apkPath = EditorUtility.OpenFilePanel("Select APK", "", "apk");
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

		private void DrawInstallButton() {
			EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(selectedDevice) || string.IsNullOrEmpty(apkPath));

			EditorGUILayout.Space();

			if (isInstalling) {
				EditorGUILayout.LabelField("Installing APK...", EditorStyles.boldLabel);
				DrawProgressBar(installationProgress);
			} else {
				if (GUILayout.Button("Install", GUILayout.Height(30))) {
					installAPK();
				}
			}

			EditorGUI.EndDisabledGroup();
		}

		private void DrawCancelButton()
        {
            EditorGUI.BeginDisabledGroup(!isInstalling);

            EditorGUILayout.Space();

            if (GUILayout.Button("Cancel", GUILayout.Height(30)))
            {
                // Cancel installation
                isInstalling = false;
                installationSuccess = false;
                installationProgress = 0f;

                // Kill the ADB process if it's still running
                KillADBProcess();
            }

            EditorGUI.EndDisabledGroup();
        }

		private async void installAPK()
		{
			isInstalling = true;
			installationSuccess = false;
			installationProgress = 0f;

			string command = $"-s {selectedDevice} install -r \"{apkPath}\"";
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = adbPath,
				Arguments = command,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};

			try
			{
				await Task.Run(() =>
				{
					using (Process process = new Process())
					{
						process.StartInfo = startInfo;
						process.OutputDataReceived += (_, e) => {
							Debug.Log(e.Data);
							if (e.Data != null && e.Data.Contains("Success")) {
								installationProgress = 1f;
							} else if (float.TryParse(e.Data, out var progress)) {
								installationProgress = progress;
							}
						};

						process.Start();
						process.BeginOutputReadLine();

						process.WaitForExit();

						// Check the exit code to determine if the installation was successful
						installationSuccess = process.ExitCode == 0;
					}
				});
			}
			catch (Exception exception)
			{
				isInstalling = false;
				ShowInstallationResult(false, $"An error occurred: {exception.Message}");
			}
			finally
			{
				isInstalling = false;
				ShowInstallationResult(installationSuccess, null);
			}
		}

		private void ShowInstallationResult(bool success, string message)
        {
            string resultMessage = success ? "APK installation succeeded." : "APK installation failed.";
            if (!string.IsNullOrEmpty(message))
            {
                resultMessage += Environment.NewLine + message;
            }

            EditorUtility.DisplayDialog("APK Installation", resultMessage, "OK");
        }

		private void DrawProgressBar(float progress)
        {
            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 30f,
                progressSliderHeight,
                EditorGUIUtility.labelWidth,
                EditorGUIUtility.labelWidth);
            EditorGUI.ProgressBar(rect, progress, $"{(int)(progress * 100)}%");

            EditorGUILayout.Space();
        }

        private void KillADBProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = adbPath,
                Arguments = "kill-server",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
