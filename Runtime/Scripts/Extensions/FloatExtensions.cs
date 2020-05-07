using System;

namespace CommonUtils.Extensions {
	public static class FloatExtensions {
		public static string SecondsToMinutesSecondsString(this float seconds) {
			var t = TimeSpan.FromSeconds(seconds);
			return $"{t.Minutes:D2}:{t.Seconds:D2}";
		}
	}
}
