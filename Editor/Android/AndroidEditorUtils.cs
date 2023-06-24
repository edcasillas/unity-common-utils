using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace CommonUtils.Editor.Android {
	public static class AndroidEditorUtils {
		public static KeyValuePair<string, string> DeviceSelector(KeyValuePair<string, string> selectedDevice, Dictionary<string, string> deviceList = null) {
			if (deviceList.IsNullOrEmpty()) {
				deviceList = ADBUtils.GetConnectedDevices();
				if (deviceList.IsNullOrEmpty()) {
					EditorGUILayout.HelpBox("No Android devices are connected", MessageType.Warning);
					return default;
				}
			}

			var deviceArray = convertDictionaryToArray(deviceList);
			var selectedIndex = string.IsNullOrEmpty(selectedDevice.Key) ?
				0 :
				Array.IndexOf(deviceArray, $"{selectedDevice.Value} ({selectedDevice.Key})");
			if (selectedIndex < 0) selectedIndex = 0;
			var newIndex = EditorGUILayout.Popup("Select Device", selectedIndex, deviceArray);
			var selectedKey = deviceList!.ElementAt(newIndex).Key;
			deviceList.TryGetValue(selectedKey, out var selectedValue);
			return new KeyValuePair<string, string>(selectedKey, selectedValue);
		}

		private static string[] convertDictionaryToArray(Dictionary<string, string> dictionary) {
			var array = new string[dictionary.Count];
			var index = 0;
			foreach (var pair in dictionary) {
				array[index] = $"{pair.Value} ({pair.Key})";
				index++;
			}

			return array;
		}
	}
}