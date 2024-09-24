using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CommonUtils.Editor.Publitch {
	internal static class ButlerParser {
		private static readonly List<string> expectedNonProgressStrings = new List<string>() {
			"For channel", // '> For channel `xxxx`: last build is xxxxx, downloading its signature'
			"Pushing", // '> Pushing xxx.xx MiB (xx files, xx dirs, xx symlinks)'
			"MiB patch", // '< xx.xx MiB patch (xx.xx% savings)'
			"Build is now processing", // '> Build is now processing, should be up in a bit.'
			"for more information", // 'Use the `butler status xxx/xxx:xxx` for more information.'
			"creating build on remote server" // 'creating build on remote server: itch.io API error (400): /wharf/builds: channel xxxx's latest build is still processing, try again later'
		};

		public enum ParseResult {
			NotFound,
			EmptyString,
			ExpectedNonProgress,
			MultiplePeriodsInNumber,
			UnexpectedCharacter,
			UnexpectedError,
			Ok
		}

		internal static ParseResult TryParseProgress(string butlerOutput, out float progress) {
			progress = 0f;
			if (string.IsNullOrEmpty(butlerOutput)) return ParseResult.EmptyString;

			if (expectedNonProgressStrings.Any(butlerOutput.Contains)) { return ParseResult.ExpectedNonProgress; }

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
								return ParseResult.MultiplePeriodsInNumber;
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
							 * 'creating build on remote server: itch.io API error (400): /wharf/builds: channel xxxx's latest build is still processing, try again later'
							 */
							//Debug.LogError($"Unexpected character in string received from butler: '{butlerOutput}'");
							return ParseResult.UnexpectedCharacter;
						}
						i++;
					}

					foundPct = true;

					if (float.TryParse(pctBuilder.ToString(), out var pct)) {
						progress = pct;
					} else {
						foundPct = false;
					}

					break;
				}
				i++;
			}

			return foundPct ? ParseResult.Ok : ParseResult.UnexpectedError;
		}
	}
}