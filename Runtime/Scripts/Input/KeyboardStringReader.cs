using System.Collections.Generic;
using System.Text;
using CommonUtils.Extensions;
using CommonUtils.Verbosables;
using UnityEngine;

namespace CommonUtils.Input {
	/// <summary>
	/// Reads a string being collected every frame through keyboard input and triggers the <see cref="onValueRead"/> event when the <see cref="submitKey"/> is detected.
	/// </summary>
	[AddComponentMenu("Input/Keyboard String Reader")]
	public class KeyboardStringReader : MonoBehaviour, IVerbosable {
		#region Inspector fields
		#pragma warning disable 649
		/// <summary>
		/// Set of characters this reader can be read. If left empty, the string won't be constrained.
		/// </summary>
		[Tooltip("Set of characters this reader can be read. If left empty, the string won't be constrained.")]
		[SerializeField] private string readableChars;

		/// <summary>
		/// Key to act as a submit button.
		/// </summary>
		[Tooltip("Key to act as a submit button.")]
		[SerializeField] private KeyCode submitKey = KeyCode.Return;

		/// <summary>
		/// Event to be executed when the <see cref="submitKey"/> is detected and the input string is not empty.
		/// </summary>
		[Tooltip("Event to be executed when the Submit Key is detected and the input string is not empty.")]
		[SerializeField] private StringEvent onValueRead;

		[SerializeField] private bool verbose;
		#pragma warning restore 649
		#endregion

		#region Private fields
		/// <summary>
		/// Input buffer.
		/// </summary>
		private readonly StringBuilder strBuilder = new StringBuilder();

		/// <summary>
		/// Internal set of valid characters for constant access time.
		/// </summary>
		private HashSet<char> readableCharsSet;
		#endregion

		public bool IsVerbose => verbose;

		private void Awake() {
			// Initializes the set of valid characters for this reader.
			if (!string.IsNullOrEmpty(readableChars)) {
				readableCharsSet = new HashSet<char>();
				for (var i = 0; i < readableChars.Length; i++) {
					if (!readableCharsSet.Contains(readableChars[i])) readableCharsSet.Add(readableChars[i]);
				}
			}
		}

		private void Update() {
			#region Read the string input every frame
			if (!string.IsNullOrEmpty(UnityEngine.Input.inputString)) {
				var inputString = UnityEngine.Input.inputString;
				if (readableCharsSet.IsNullOrEmpty()) { // If no readable characters have been specified, read everything.
					strBuilder.Append(inputString);
				}
				else { // otherwise check character by character validating it's not restricted.
					for (int i = 0; i < inputString.Length; i++) {
						if (readableCharsSet.Contains(inputString[i])) strBuilder.Append(inputString[i]);
					}
				}
			}
			#endregion

			#region Submit when the submit key is detected and the input string is not empty.
			if (UnityEngine.Input.GetKeyUp(submitKey) && strBuilder.Length > 0) {
				var readValue = strBuilder.ToString().TrimEnd();
				this.DebugLog($"{name} read the string \"{readValue}\"");
				onValueRead.Invoke(readValue);
				strBuilder.Clear();
			}
			#endregion
		}
	}
}