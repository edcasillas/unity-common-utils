using System;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class ColorExtensions {
		public static string ColorToHex(this Color color)
			=> color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");

		public static Color HexToColor(this string inputHexString) {
			if (string.IsNullOrEmpty(inputHexString)) throw new ArgumentNullException(nameof(inputHexString));
			if (inputHexString.Length != 6)
				throw new ArgumentException("Input string must have exactly 6 characters.", nameof(inputHexString));

			var r = byte.Parse(inputHexString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			var g = byte.Parse(inputHexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			var b = byte.Parse(inputHexString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r, g, b, 255);
		}
	}
}
