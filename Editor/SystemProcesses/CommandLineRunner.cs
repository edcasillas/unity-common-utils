using CommonUtils.Verbosables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace CommonUtils.Editor.SystemProcesses {
	public class CommandLineRunner : IVerbosable {
		private Process process;
		private string lastCommandExecuted;

		private Action<string> onOutputDataReceived;
		private Action<Win32ErrorCode, string> onFailed;
		private Action<string> onSuccess;
		private Action onFinished;

		private DateTime? startTime;
		private DateTime? endTime;

		public bool IsRunning => process is { HasExited: false };

		public TimeSpan? ExecutionTime {
			get {
				if (startTime == null) return null; // Process has never been started
				if (IsRunning) return DateTime.Now - startTime; // Process is still running
				return endTime - startTime; // Process finished, return total time
			}
		}

		public Verbosity Verbosity { get; set; } = Verbosity.Warning | Verbosity.Error;

		public bool Run(string command, string args = null, string commandPath = null, Action<string> onOutputDataReceived = null, Action<string> onSuccess = null, Action<Win32ErrorCode, string> onFailed = null, Action onFinished = null, IDictionary<string, string> environmentVariables = null) {
			if (process != null) {
				if (process.HasExited) {
					cleanup();
				} else {
					this.LogNoContext($"Process {lastCommandExecuted} is already running.", LogLevel.Error);
					return false;
				}
			}

			// Prepare the process start info
			var startInfo = new ProcessStartInfo {
				FileName = string.IsNullOrEmpty(commandPath) ? command : Path.Combine(commandPath, command),
				Arguments = args ?? string.Empty,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			// Apply environment variables
			if (environmentVariables != null) {
				foreach (var environmentVariable in environmentVariables) {
					startInfo.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value;
				}
			}

			this.onOutputDataReceived = onOutputDataReceived;
			this.onSuccess = onSuccess;
			this.onFailed = onFailed;
			this.onFinished = onFinished;

			process = new Process { StartInfo = startInfo };
			if (onOutputDataReceived != null) process.OutputDataReceived += outputDataReceivedHandler;

			try {
				startTime = DateTime.Now;
				process.Start();
				lastCommandExecuted = command;
				if (onOutputDataReceived != null) process.BeginOutputReadLine();
				EditorApplication.update += update;
				this.LogNoContext($"Started process \"{startInfo.FileName}\" with args {process.StartInfo.Arguments}.");
				return true;
			} catch (Win32Exception ex) {
				this.LogNoContext($"Failed to start process \"{startInfo.FileName}\" with error {ex.Message}.", LogLevel.Error);
				startTime = endTime = null;
				onFailed?.Invoke((Win32ErrorCode)ex.NativeErrorCode, ex.Message);
				cleanup();
				return false;
			}
		}

		public void Kill() {
			if (process == null || process.HasExited) {
				this.LogNoContext($"Process {lastCommandExecuted} could not be killed because it has already exited or has not been started.");
				return;
			}

			try {
				process.Kill();
				// Optionally wait for process to exit after kill command.
				// process.WaitForExit();
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
			endTime = DateTime.Now; // Mark end time when process finishes

			if (process.ExitCode == 0) {
				// Process finished successfully
				var output = onOutputDataReceived == null ? process.StandardOutput.ReadToEnd() : null;
				onSuccess?.Invoke(output);
			} else {
				// Process finished with an error
				var error = process.StandardError.ReadToEnd();
				if (string.IsNullOrEmpty(error)) error = process.StandardOutput.ReadToEnd();

				onFailed?.Invoke((Win32ErrorCode)process.ExitCode, error);
			}

			onFinished?.Invoke();

			if (onOutputDataReceived != null) process.OutputDataReceived -= outputDataReceivedHandler;
			cleanup(); // Clean up the process but don't clear the startTime or endTime
		}

		private void outputDataReceivedHandler(object sender, DataReceivedEventArgs args) {
			if (string.IsNullOrWhiteSpace(args?.Data)) return; // Ignore empty lines.
			onOutputDataReceived?.Invoke(args.Data);
		}

		private void cleanup() {
			// Only dispose of the process, but keep the timing data intact
			process?.Dispose();
			process = null;
		}
	}
}
