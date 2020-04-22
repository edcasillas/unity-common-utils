using System;
using UnityEngine;

namespace CommonUtils {
	/// <summary>
	/// Contains extension methods to load and save images from Base64 strings.
	/// </summary>
	public static class Base64ImageExtensions {
		/// <summary>
		/// Converts a base64 string to <see cref="Texture2D"/>.
		/// </summary>
		/// <param name="base64string">Source base64 string.</param>
		/// <returns><see cref="Texture2D"/> loaded from the base64 string.</returns>
		public static Texture2D ToTexture2DFromBase64String(this string base64string) {
			if (string.IsNullOrEmpty(base64string)) return null;
			var result = new Texture2D(1, 1);
			try {
				var bytes = Convert.FromBase64String(base64string);
				result.LoadImage(bytes);
				result.Apply();
				return result;
			}
			catch (Exception ex) {
				Debug.LogErrorFormat("Unable to convert string to Texture 2D: {0}", ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Converts a <see cref="Texture2D"/> to its base64 string representation.
		/// </summary>
		/// <param name="texture">Source <see cref="Texture2D"/></param>
		/// <returns>Base64 representation of <paramref name="texture"/>.</returns>
		public static string ToBase64String(this Texture2D texture) {
			if (texture == null) return null;
			string result = null;
			try {
				result = Convert.ToBase64String(texture.EncodeToPNG());
			}
			catch (UnityException ex) {
				Debug.LogErrorFormat("Couldn't convert texture to Base64: {0}", ex.Message);
			}

			return result;
		}
	}
}