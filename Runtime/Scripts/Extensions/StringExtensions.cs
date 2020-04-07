using System.Text;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class StringExtensions {
		public static void CopyToClipboard(this string input) {
			var textEditor = new TextEditor {text = input};
			textEditor.SelectAll();
			textEditor.Copy();
			Debug.Log($"Text {input} has been copied to clipboard.");
		}

		/// <summary>
		/// Returns a string with the specified count of white spaces.
		/// </summary>
		public static string GetWhiteSpaces(int count) {
			var result = new StringBuilder();
			for (var i = 0; i < count; i++) {
				result.Append(" ");
			}

			return result.ToString();
		}
	}
}