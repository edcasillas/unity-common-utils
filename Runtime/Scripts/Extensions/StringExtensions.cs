using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class StringExtensions {
		public static void CopyToClipboard(this string input) {
			var textEditor = new TextEditor {text = input};
			textEditor.SelectAll();
			textEditor.Copy();
			Debug.Log($"Text {input} has been copied to clipboard.");
		}

		public static string GetFromClipboard() {
			var textEditor = new TextEditor();
			textEditor.Paste();
			return textEditor.text;
		}

		/// <summary>
		/// Returns a string with the specified count of white spaces.
		/// </summary>
		public static string GetWhiteSpaces(this int count) {
			var result = new StringBuilder();
			for (var i = 0; i < count; i++) {
				result.Append(" ");
			}

			return result.ToString();
		}

		public static string FirstCharToUpper(this string input) {
			switch (input) {
				case null: throw new ArgumentNullException(nameof(input));
				case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
				default: return input.First().ToString().ToUpper() + input.Substring(1);
			}
		}

		public static string FirstCharToLower(this string input) {
			switch (input) {
				case null: throw new ArgumentNullException(nameof(input));
				case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
				default: return input.First().ToString().ToLower() + input.Substring(1);
			}
		}

		public static string PascalToTitleCase(this string text) {
			text = text.Substring(0, 1).ToUpper() + text.Substring(1);
			return Regex.Replace(text, @"(\B[A-Z])", @" $1");
		}
	}
}