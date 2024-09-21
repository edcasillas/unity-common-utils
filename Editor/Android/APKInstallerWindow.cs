using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonUtils.Editor.Android {
    public class APKInstallerWindow : EditorWindow {
        private const string menuPath = "Tools/Android/Install APK...";

        private string selectedDeviceId = string.Empty;
        private string selectedDeviceName = string.Empty;
        private string apkPath = string.Empty;
        private bool isInstalling = false;
        private bool installationSuccess = false;
        private bool forceOverride = true;
        private bool installOnAllDevices = false;
		private string installOutputText;

        [MenuItem(menuPath)]
        private static void openWindow() {
            var window = GetWindow<APKInstallerWindow>();
            window.titleContent = new GUIContent("Install APK");
            window.minSize = new Vector2(400f, 200f);
            window.Show();
        }

        private void OnGUI() {
            EditorGUILayout.Space();

            if (!ADBUtils.IsADBInstalled()) {
                EditorGUILayout.HelpBox(
                    "ADB is not installed. Please make sure Android SDK is correctly installed and configured.",
                    MessageType.Error);
                return;
            }

            var connectedDevices = ADBUtils.GetConnectedDevices();
            var selectedDevice = new KeyValuePair<string, string>(selectedDeviceId, selectedDeviceName);
            selectedDevice = AndroidEditorUtils.DeviceSelector(selectedDevice, connectedDevices);
            selectedDeviceId = selectedDevice.Key;
            selectedDeviceName = selectedDevice.Value;

            drawAPKField();

            forceOverride = EditorGUILayout.Toggle("Force override (downgrade)", forceOverride);
			if (connectedDevices.Count > 1) {
				installOnAllDevices = EditorGUILayout.Toggle("Install on all devices", installOnAllDevices);
			}

            drawInstallButton();
            drawCancelButton();

			if (!isInstalling && !string.IsNullOrWhiteSpace(installOutputText)) {
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox(
					installationSuccess ? "Last install was a success." : "Last install was an error.",
					installationSuccess ? MessageType.Info : MessageType.Error);
				EditorGUILayout.TextArea(installOutputText);
			}
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
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(apkPath));

            EditorGUILayout.Space();

            if (isInstalling) {
                this.ShowLoadingSpinner($"Installing {Path.GetFileName(apkPath)}{(installOnAllDevices ? " on all devices" : "")}...");
            } else {
                if (GUILayout.Button($"Install {Path.GetFileName(apkPath)}{(installOnAllDevices ? " on all devices" : "")}", GUILayout.Height(30))) {
                    if (installOnAllDevices) {
                        InstallAPKOnAllDevicesAsync();
                    } else {
                        installAPKAsync();
                    }
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        private void drawCancelButton() {
            if (!isInstalling) return;

            EditorGUILayout.Space();

            if (GUILayout.Button("Cancel", GUILayout.Height(30))) {
                // Cancel installation
                isInstalling = false;
                installationSuccess = false;
				installOutputText = "Installation was cancelled";

                // Kill the ADB process if it's still running
                ADBUtils.KillADBProcess();
            }
        }

        private async void installAPKAsync() {
            isInstalling = true;
            installationSuccess = false;
			installOutputText = null;

            var command = $"-s {selectedDeviceId} install -r{(forceOverride ? " -d" : "")} \"{apkPath}\"";
            var installOutput = await ADBUtils.RunADBCommandAsync(command);

            installationSuccess = installOutput.exitCode == 0;

            var resultMessage = installationSuccess
                ? $"{Path.GetFileName(apkPath)} was successfully installed on {selectedDeviceName}"
                : $"Could not install {Path.GetFileName(apkPath)} on {selectedDeviceName}. Exit code = {installOutput.exitCode}";

            showInstallationResult(installationSuccess, resultMessage, new List<string> { selectedDeviceName });
            isInstalling = false;
			installOutputText = installOutput.output;
        }

        private async void InstallAPKOnAllDevicesAsync() {
            isInstalling = true;
            installationSuccess = false;
			installOutputText = null;

            var connectedDevices = ADBUtils.GetConnectedDevices();
            var commands = connectedDevices.Select(device => $"-s {device.Key} install -r{(forceOverride ? " -d" : "")} \"{apkPath}\"").ToList();
            var installOutputs = await RunADBCommandsAsync(commands, connectedDevices.Values.ToList());

            var exitCodes = installOutputs.ToDictionary(output => output.deviceName, output => output.exitCode);

            var successDevices = exitCodes.Where(kv => kv.Value == 0).Select(kv => kv.Key).ToList();
            var failureDevices = exitCodes.Where(kv => kv.Value != 0).Select(kv => kv.Key).ToList();

            var successMessage = $"{Path.GetFileName(apkPath)} was successfully installed on all devices:\n{string.Join("\n", successDevices)}";
            var failureMessage = $"Could not install {Path.GetFileName(apkPath)} on some devices. Devices:\n{string.Join("\n", failureDevices)}";
            showInstallationResult(failureDevices.Count == 0, failureDevices.Count == 0 ? successMessage : failureMessage, successDevices);

            isInstalling = false;
			installOutputText = string.Join("\n\n", installOutputs.Select(output => output.output));
        }

		private async Task<List<(string output, int exitCode, string deviceName)>> RunADBCommandsAsync(List<string> commands, List<string> deviceNames) {
            var outputs = new List<(string output, int exitCode, string deviceName)>();

            for (int i = 0; i < commands.Count; i++) {
                var command = commands[i];
                var deviceName = deviceNames[i];

                var output = await ADBUtils.RunADBCommandAsync(command);
                outputs.Add((output.output, output.exitCode, deviceName));
            }

            return outputs;
        }

        private void showInstallationResult(bool success, string message, List<string> deviceList = null) {
            var resultMessage = success ? "APK installation succeeded." : "APK installation failed.";

            if (deviceList != null && deviceList.Any()) {
                resultMessage += $" Devices: {string.Join(", ", deviceList)}";
            }

            resultMessage += Environment.NewLine + message;

            EditorUtility.DisplayDialog("APK Installation", resultMessage, "OK");
        }
	}
}
