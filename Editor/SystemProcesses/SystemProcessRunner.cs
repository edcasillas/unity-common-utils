using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace CommonUtils.Editor.SystemProcesses
{
	public class SystemProcessRunner {
		private readonly ProcessStartInfo startInfo;
		private Process process;

		private Action<string> onOutputDataReceived;
		private Action<Win32ErrorCode, string> onFailed;
		private Action<string> onSuccess;

		public bool IsRunning => process is { HasExited: false };

		public SystemProcessRunner(string command, string args, string commandPath = null, Action<string> onOutputDataReceived = null, Action<string> onSuccess = null, Action<Win32ErrorCode, string> onFailed = null, IDictionary<string, string> environmentVariables = null) {
			var processFileName = string.IsNullOrEmpty(commandPath) ? command : Path.Combine(commandPath, command);

			startInfo = new ProcessStartInfo {
				FileName = processFileName,
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			if (!environmentVariables.IsNullOrEmpty()) {
				foreach (var environmentVariable in environmentVariables!) {
					startInfo.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
				}
			}

			this.onOutputDataReceived = onOutputDataReceived;
			this.onSuccess = onSuccess;
			this.onFailed = onFailed;
		}

		public void Start() {
			if (process != null) {
				Debug.LogError("Already running.");
				return;
			}

			process = new Process() { StartInfo = startInfo };
			if (onOutputDataReceived != null) process.OutputDataReceived += outputDataReceivedHandler;

			try {
				process.Start();
				if (onOutputDataReceived != null) process.BeginOutputReadLine();
				EditorApplication.update += update;
			} catch (Win32Exception ex) {
				Debug.LogException(ex);
				onFailed?.Invoke((Win32ErrorCode)ex.NativeErrorCode, ex.Message);
				cleanup();
			}
		}

		public void Kill() {
			if (process == null || process.HasExited) {
				Debug.LogWarning("Process is not running or has already exited.");
				return;
			}

			try {
				process.Kill();
				process.WaitForExit(); // Optional: Wait for process to exit after kill command.
				Debug.Log("Process killed successfully.");
			} catch (InvalidOperationException ex) {
				Debug.LogError($"Failed to kill process: {ex.Message}");
			} catch (Win32Exception ex) {
				Debug.LogError($"Win32Exception occurred while killing the process: {ex.Message}");
				onFailed?.Invoke((Win32ErrorCode)ex.NativeErrorCode, ex.Message);
			} finally {
				cleanup();
			}
		}

		private void update() {
			if (process is not { HasExited: true }) return;
			EditorApplication.update -= update;

			if (process.ExitCode == 0) {
				// Process finished successfully
				onSuccess?.Invoke(process.StandardOutput.ReadToEnd());
			} else {
				// Process finished with an error
				var error = process.StandardError.ReadToEnd();
				onFailed?.Invoke((Win32ErrorCode)process.ExitCode, error);
			}

			if (onOutputDataReceived != null) process.OutputDataReceived -= outputDataReceivedHandler;
			cleanup();
		}

		private void outputDataReceivedHandler(object sender, DataReceivedEventArgs args) {
			if(string.IsNullOrWhiteSpace(args?.Data)) return; // Ignore empty lines.
			onOutputDataReceived?.Invoke(args.Data);
		}

		private void cleanup() {
			process?.Dispose();
			process = null;
		}
	}
}