using System;
using UnityEngine;

namespace CommonUtils.LocalPersistence {
	public static class PlayerPrefsUtils {
		public static void SetVector2(string key, Vector2 value) {
			PlayerPrefs.SetFloat($"{key}_x", value.x);
			PlayerPrefs.SetFloat($"{key}_y", value.y);
		}

		public static Vector2 GetVector2(string key) => new Vector2(PlayerPrefs.GetFloat($"{key}_x"), PlayerPrefs.GetFloat($"{key}_y"));

		public static void SetDateTime(string key, DateTime dateTime) => PlayerPrefs.SetString(key, dateTime.Ticks.ToString());

		public static DateTime GetDateTime(string key, DateTime dateTime, DateTime? defaultValue = null) {
			var ticksStr = PlayerPrefs.GetString(key);
			if (string.IsNullOrEmpty(ticksStr) || !long.TryParse(ticksStr, out var result)) {
				return defaultValue ?? DateTime.UnixEpoch;
			}
			return new DateTime(result);
		}
	}
}