using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor.Android;
using UnityEngine;

namespace CommonUtils.Editor.Android {
    public static class ADBUtils {
        private static string adbPath => Path.Combine(AndroidExternalToolsSettings.sdkRootPath, "platform-tools", getADBExecutableName());

        public static bool IsADBInstalled() {
            var adb = adbPath;
            if (string.IsNullOrEmpty(adb))
                return false;

            var (_, exitCode) = runADBCommand("version");
            return exitCode == 0;
        }

        public static Dictionary<string, string> GetConnectedDevices() {
            var devices = new Dictionary<string, string>();

            var adbPath = ADBUtils.adbPath;
            if (string.IsNullOrEmpty(adbPath))
                return devices;

            var (output, _) = runADBCommand("devices -l");

			using var reader = new StringReader(output);
			string line;
			while ((line = reader.ReadLine()) != null) {
				if (string.IsNullOrWhiteSpace(line) || line == "List of devices attached") continue;
				var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (split.Length >= 4) {
					var id = split[0];
					var model = string.Empty;
					for (var i = 1; i < split.Length; i++) {
						if (split[i].StartsWith("model:")) {
							model = split[i].Replace("model:", string.Empty).Replace("_", " ");
						}
					}
					devices[id] = model;
				}
			}

			return devices;
        }

        public static async Task<(string output, int exitCode)> RunADBCommandAsync(string command) => await Task.Run(() => {
			var startInfo = new ProcessStartInfo {
				FileName = ADBUtils.adbPath,
				Arguments = command,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};

			using var process = new Process();
			process.StartInfo = startInfo;

			var output = new System.Text.StringBuilder();
			process.OutputDataReceived += (_, e) => {
				output.AppendLine(e.Data);
			};

			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();

			var exitCode = process.ExitCode;

			return (output.ToString(), exitCode);
		});

		public static void KillADBProcess() {
            var startInfo = new ProcessStartInfo {
                FileName = adbPath,
                Arguments = "kill-server",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            runProcess(startInfo);
        }

        private static void runProcess(ProcessStartInfo startInfo) {
            using (var process = new Process()) {
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
        }

        private static (string output, int exitCode) runADBCommand(string command) {
            var startInfo = new ProcessStartInfo {
                FileName = adbPath,
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

			using var process = new Process();
			process.StartInfo = startInfo;

			var output = new System.Text.StringBuilder();
			process.OutputDataReceived += (_, e) => {
				output.AppendLine(e.Data);
			};

			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();

			var exitCode = process.ExitCode;

			return (output.ToString(), exitCode);
		}

        private static string getADBExecutableName() => Application.platform switch {
            RuntimePlatform.WindowsEditor => "adb.exe",
            RuntimePlatform.OSXEditor => "adb",
            RuntimePlatform.LinuxEditor => "adb",
            _ => throw new NotSupportedException("Unsupported platform."),
        };
    }
}
