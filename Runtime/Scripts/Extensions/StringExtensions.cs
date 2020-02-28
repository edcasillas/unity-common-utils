using UnityEngine;

namespace CommonUtils.Extensions {
	public static class StringExtensions {
		public static void CopyToClipboard(this string input) {
			var textEditor = new TextEditor {text = input};
			textEditor.SelectAll();
			textEditor.Copy();
			Debug.Log($"Text {input} has been copied to clipboard.");
		}
	}
}