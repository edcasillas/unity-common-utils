using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

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
            var selectedDevice = new KeyValuePair<string, string>(selectedDeviceId, selectedDeviceName);
            selectedDevice = AndroidEditorUtils.DeviceSelector(selectedDevice, connectedDevices);
            selectedDeviceId = selectedDevice.Key;
            selectedDeviceName = selectedDevice.Value;

            drawAPKField();

            forceOverride = EditorGUILayout.Toggle("Force override (downgrade)", forceOverride);
            installOnAllDevices = EditorGUILayout.Toggle("Install on all devices", installOnAllDevices);

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

                // Kill the ADB process if it's still running
                killADBProcess();
            }
        }

        private async void installAPKAsync() {
            isInstalling = true;
            installationSuccess = false;

            var command = $"-s {selectedDeviceId} install -r{(forceOverride ? " -d" : "")} \"{apkPath}\"";
            var startInfo = new ProcessStartInfo {
                FileName = AndroidEditorUtils.ADBPath,
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var installOutput = new System.Text.StringBuilder();
            installOutput.AppendLine($"adb {command}");
            var exitCode = 0;

            try {
                await System.Threading.Tasks.Task.Run(() => {
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
                        exitCode = process.ExitCode;
                    }
                });
                showInstallationResult(installationSuccess,
                    installationSuccess ? $"{Path.GetFileName(apkPath)} was successfully installed on {selectedDeviceName}" : $"Could not install {Path.GetFileName(apkPath)} on {selectedDeviceName}. Exit code = {exitCode}",
                    new List<string> { selectedDeviceName });
            } catch (Exception exception) {
                showInstallationResult(false, $"An error occurred: {exception.Message}");
            } finally {
                isInstalling = false;
                Debug.Log(installOutput.ToString());
            }
        }

        private async void InstallAPKOnAllDevicesAsync() {
            isInstalling = true;
            installationSuccess = false;

            var connectedDevices = AndroidEditorUtils.GetConnectedDevices();
            var commands = new List<string>();

            foreach (var device in connectedDevices) {
                var deviceId = device.Key;
                var command = $"-s {deviceId} install -r{(forceOverride ? " -d" : "")} \"{apkPath}\"";
                commands.Add(command);
            }

            var installOutput = new System.Text.StringBuilder();
            var exitCodes = new Dictionary<string, int>();

            try {
                await System.Threading.Tasks.Task.Run(() => {
                    foreach (var command in commands) {
                        var startInfo = new ProcessStartInfo {
                            FileName = AndroidEditorUtils.ADBPath,
                            Arguments = command,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };

                        var deviceName = connectedDevices[selectedDeviceId];
                        installOutput.AppendLine($"adb {command}");
                        var exitCode = 0;

                        using (var process = new Process()) {
                            process.StartInfo = startInfo;
                            process.OutputDataReceived += (_, e) => {
                                installOutput.AppendLine(e.Data);
                            };

                            process.Start();
                            process.BeginOutputReadLine();

                            process.WaitForExit();

                            // Check the exit code to determine if the installation was successful
                            exitCode = process.ExitCode;
							if (!exitCodes.TryAdd(deviceName, exitCode)) exitCodes[deviceName] = exitCode;
						}
                    }
                });

                var successDevices = exitCodes.Where(kv => kv.Value == 0).Select(kv => kv.Key).ToList();
                var failureDevices = exitCodes.Where(kv => kv.Value != 0).Select(kv => kv.Key).ToList();

                var successMessage = $"{Path.GetFileName(apkPath)} was successfully installed on all devices:\n{string.Join("\n", successDevices)}";
                var failureMessage = $"Could not install {Path.GetFileName(apkPath)} on some devices. Devices:\n{string.Join("\n", failureDevices)}";
                showInstallationResult(failureDevices.Count == 0, failureDevices.Count == 0 ? successMessage : failureMessage, successDevices);
            } catch (Exception exception) {
                showInstallationResult(false, $"An error occurred: {exception.Message}");
            } finally {
                isInstalling = false;
                Debug.Log(installOutput.ToString());
            }
        }

        private void showInstallationResult(bool success, string message, List<string> deviceList = null) {
            var resultMessage = success ? "APK installation succeeded." : "APK installation failed.";

            if (deviceList != null && deviceList.Any()) {
                resultMessage += $" Devices: {string.Join(", ", deviceList)}";
            }

            resultMessage += Environment.NewLine + message;

            EditorUtility.DisplayDialog("APK Installation", resultMessage, "OK");
        }

        private static void killADBProcess() {
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
