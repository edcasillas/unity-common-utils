using System;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class ColorExtensions {
		public static string ColorToHex(this Color color, bool includeAlpha = false) {
			Color32 color32 = color;
			var result = color32.r.ToString ("X2") + color32.g.ToString ("X2") + color32.b.ToString ("X2");
			if (includeAlpha) result += color32.a.ToString("X2");
			return result;
		}

		public static Color HexToColor(this string inputHexString) {
			if (string.IsNullOrEmpty(inputHexString)) throw new ArgumentNullException(nameof(inputHexString));
			if (inputHexString.Length != 6 && inputHexString.Length != 8)
				throw new ArgumentException("Input string must have exactly 6 or 8 characters (without or with alpha).", nameof(inputHexString));

			var r = byte.Parse(inputHexString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			var g = byte.Parse(inputHexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			var b = byte.Parse(inputHexString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			var a = inputHexString.Length == 8
				? byte.Parse(inputHexString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
				: (byte)255;
			return new Color32(r, g, b, a);
		}
		
		public static Color GetColorFromRGB255(int r, int g, int b) => new Color (r / 255.0f, g / 255.0f, b / 255.0f);
	}
}
