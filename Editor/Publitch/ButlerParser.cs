using System.Text;
using UnityEngine;

namespace CommonUtils.Editor.Publitch {
	internal static class ButlerParser {
		internal static bool TryParseStatus(string butlerOutput, out string channelName, out string upload, out string build, out string version) {
			channelName = null;
			upload = null;
			build = null;
			version = null;

			if (string.IsNullOrEmpty(butlerOutput)) return false;

			var lines = butlerOutput.Split("\n");
			if (lines.Length < 4) return false;

			var columns = lines[3].Split("|");
			if (columns.Length < 4) return false;

			channelName = columns[1].Trim();
			upload = columns[2].Trim();
			build = columns[3].Trim();
			version = columns[4].Trim();

			return true;
		}

		internal static bool TryParseProgress(string butlerOutput, out float progress) {
			progress = 0f;
			if (string.IsNullOrEmpty(butlerOutput)) return false;
			var foundPct = false;

			var i = 0;
			while (i < butlerOutput.Length) {
				var num = (int)char.GetNumericValue(butlerOutput[i]);
				if (num >= 0) {
					var pctBuilder = new StringBuilder();
					pctBuilder.Append(num);
					var foundPeriod = false;
					i++;
					while (i < butlerOutput.Length && butlerOutput[i] != '%') {
						num = (int)char.GetNumericValue(butlerOutput[i]);
						if (num>=0) {
							pctBuilder.Append(num);
						} else if (butlerOutput[i] == '.') {
							if (foundPeriod) {
								Debug.LogError($"Bad percentage info in butler string: {butlerOutput}");
								return false;
							}
							pctBuilder.Append(".");
							foundPeriod = true;
						} else {
							Debug.LogError($"Unexpected character in string received from butler: '{butlerOutput}'");
							return false;
						}
						i++;
					}

					foundPct = true;

					if (float.TryParse(pctBuilder.ToString(), out var pct)) {
						progress = pct;
					}

					break;
				}
				i++;
			}

			return foundPct;
		}
	}
}