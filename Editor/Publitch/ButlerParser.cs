using System.Text;
using UnityEngine;

namespace CommonUtils.Editor.Publitch {
	internal static class ButlerParser {
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
							/*
							 * Some other expected strings we need to handle:
							 * '> For channel `xxxx`: last build is xxxxx, downloading its signature'
							 * '> Pushing xxx.xx MiB (xx files, xx dirs, xx symlinks)'
							 * '< xx.xx MiB patch (xx.xx% savings)'
							 * '> Build is now processing, should be up in a bit.'
							 * 'Use the `butler status user/project:channel` for more information.'
							 */
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