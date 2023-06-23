using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CommonUtils.Editor.Android {
	public static class AndroidEditorUtils {
		public static string ADBPath => Path.Combine(AndroidExternalToolsSettings.sdkRootPath, "platform-tools", getADBExecutableName());
		
		public static bool IsADBInstalled() {
			var adbPath = ADBPath;
			if (string.IsNullOrEmpty(adbPath))
				return false;

			var process = new Process();
			process.StartInfo.FileName = adbPath;
			process.StartInfo.Arguments = "version";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;

			process.Start();
			process.WaitForExit();

			var exitCode = process.ExitCode;
			process.Close();

			return exitCode == 0;
		}

		private static string getADBExecutableName() => Application.platform switch {
			RuntimePlatform.WindowsEditor => "adb.exe",
			RuntimePlatform.OSXEditor => "adb",
			RuntimePlatform.LinuxEditor => "adb",
			_ => throw new NotSupportedException("Unsupported platform."),
		};

		public static Dictionary<string, string> GetConnectedDevices() {
			var devices = new Dictionary<string, string>();

			var adbPath = ADBPath;
			if (string.IsNullOrEmpty(adbPath))
				return devices;

			var process = new Process();
			process.StartInfo.FileName = adbPath;
			process.StartInfo.Arguments = "devices -l";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;

			process.Start();
			process.WaitForExit();

			while (!process.StandardOutput.EndOfStream) {
				var line = process.StandardOutput.ReadLine();
				if (string.IsNullOrWhiteSpace(line) || line == "List of devices attached") continue;
				var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (split.Length >= 4) {
					var id = split[0];
					string model = string.Empty;
					for (int i = 1; i < split.Length; i++) {
						if (split[i].StartsWith("model:")) {
							model = split[i].Replace("model:", string.Empty).Replace("_", " ");
						}
					}
					devices[id] = model;
				}
			}

			process.Close();

			return devices;
		}

		public static KeyValuePair<string, string> DeviceSelector(KeyValuePair<string, string> selectedDevice, Dictionary<string, string> deviceList)
		{
			if (deviceList.IsNullOrEmpty())
			{
				EditorGUILayout.HelpBox("No Android devices are connected", MessageType.Warning);
				return default;
			}

			var deviceArray = ConvertDictionaryToArray(deviceList);
			var selectedIndex = string.IsNullOrEmpty(selectedDevice.Key) ? 0 : Array.IndexOf(deviceArray, $"{selectedDevice.Value} ({selectedDevice.Key})");
			if (selectedIndex < 0) selectedIndex = 0;
			var newIndex = EditorGUILayout.Popup("Select Device", selectedIndex, deviceArray);
			var selectedKey = deviceList.ElementAt(newIndex).Key;
			deviceList.TryGetValue(selectedKey, out var selectedValue);
			return new KeyValuePair<string, string>(selectedKey, selectedValue);
		}

		private static string[] ConvertDictionaryToArray(Dictionary<string, string> dictionary)
		{
			var array = new string[dictionary.Count];
			var index = 0;
			foreach (var pair in dictionary)
			{
				array[index] = $"{pair.Value} ({pair.Key})";
				index++;
			}

			return array;
		}


	}
}
