using System;

namespace CommonUtils.Extensions {
	public static class DateTimeExtensions {
		private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static double ToUnixTimestamp(this DateTime date) {
			var diff = date.ToUniversalTime() - unixEpoch;
			return Math.Floor(diff.TotalSeconds);
		}

		public static DateTime FromUnixTimeStampToDate(this double timestamp) => unixEpoch.AddSeconds(timestamp);
	}
}