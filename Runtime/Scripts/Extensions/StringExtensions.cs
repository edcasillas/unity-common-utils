using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class StringExtensions {
		public static void CopyToClipboard(this string input) {
			var textEditor = new TextEditor { text = input };
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

		/// <summary>
		/// Converts a camelCase or PascalCase string to kebab-case (all lowercase with words separated by hypens).
		/// </summary>
		/// <remarks>
		/// Special characters and non-alphanumeric characters are replaced with hyphens, and sequences of consecutive
		/// special characters are collapsed into a single hyphen.
		///
		/// ALL_CAPS acronyms are supported; for each uppercase character (except the first character in the string), the
		/// method will insert a preceding hypen when it is either preceded or followed by a lowercase character.
		/// </remarks>
		/// <param name="input">A string in camelCase or PascalCase</param>
		/// <returns>kebab-case representation of the <paramref name="input"/>.</returns>
		public static string ToKebabCase(this string input) {
			if (string.IsNullOrEmpty(input)) {
				return input;
			}

			var resultBuilder = new StringBuilder();

			for (var i = 0; i < input.Length; i++) {
				var currentChar = input[i];

				// Anything that's not a letter or number is considered a special character and will be replaced with hypen.
				if (!char.IsLetterOrDigit(currentChar)) {
					// Special characters are ignored at the beginning of the string
					if (resultBuilder.Length == 0) continue;

					// For any sequence of special characters, we will only write one hypen. This line advances the iterator
					// to the last special character in the sequence.
					while (i + 1 < input.Length && !char.IsLetterOrDigit(input[i + 1])) i++;

					// Only write a hypen when we are not at the end of the string.
					if (i + 1 < input.Length) resultBuilder.Append('-');
					continue;
				}

				if (char.IsUpper(currentChar)) {
					if (
						resultBuilder.Length > 0 &&
						( // starting from the second character (won't add hypen at the beginning)
							(i + 1 < input.Length && char.IsLower(input[i + 1])) || // followed by a lowercase
							char.IsLower(input[i - 1]) // or preceded by a lowercase
						)
					) {
						resultBuilder.Append('-'); // Insert a preceding hypen
					}

					resultBuilder.Append(char.ToLower(currentChar)); // insert the character in lowercase
				} else {
					resultBuilder.Append(currentChar);
				}
			}

			return resultBuilder.ToString();
		}
	}
}