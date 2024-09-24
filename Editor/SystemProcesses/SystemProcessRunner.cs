using CommonUtils.Extensions;
using CommonUtils.Verbosables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace CommonUtils.Editor.SystemProcesses {
	public class SystemProcessRunner : IVerbosable {
		private readonly ProcessStartInfo startInfo;
		private Process process;

		private Action<string> onOutputDataReceived;
		private Action<Win32ErrorCode, string> onFailed;
		private Action<string> onSuccess;

		public bool IsRunning => process is { HasExited: false };
		public Verbosity Verbosity { get; set; } = Verbosity.Warning | Verbosity.Error;

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

		public bool Start() {
			if (process != null) {
				this.LogNoContext($"Process {process.ProcessName} is already running.", LogLevel.Error);
				return false;
			}

			process = new Process() { StartInfo = startInfo };
			if (onOutputDataReceived != null) process.OutputDataReceived += outputDataReceivedHandler;

			try {
				process.Start();
				if (onOutputDataReceived != null) process.BeginOutputReadLine();
				EditorApplication.update += update;
				this.LogNoContext($"Started process {process.ProcessName} with args {process.StartInfo.Arguments}.");
				return true;
			} catch (Win32Exception ex) {
				this.LogNoContext($"Failed to start process {process.ProcessName} with error {ex.Message}.", LogLevel.Error);
				onFailed?.Invoke((Win32ErrorCode)ex.NativeErrorCode, ex.Message);
				cleanup();
				return false;
			}
		}

		public void Kill() {
			if (process == null || process.HasExited) {
				this.LogNoContext($"Process {startInfo.FileName} could not be killed because it has already exited or has not been started.");
				return;
			}

			try {
				process.Kill();
				//process.WaitForExit(); // Optional: Wait for process to exit after kill command.
				this.LogNoContext($"Process {process.ProcessName} has been killed.");
			} catch (InvalidOperationException ex) {
				this.LogNoContext($"Failed to kill process {process.ProcessName}: {ex.Message}.", LogLevel.Error);
			} catch (Win32Exception ex) {
				this.LogNoContext($"Win32Exception occurred while killing process {process.ProcessName}: {ex.Message}.", LogLevel.Error);
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