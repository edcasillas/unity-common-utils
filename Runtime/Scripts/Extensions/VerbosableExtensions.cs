using System;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class VerbosableExtensions {
		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, string message)
			where TVerbosable : MonoBehaviour, IVerbosable {
			if (verbosable.IsVerbose) Debug.Log(message, verbosable);
		}
		
		/// <summary>
		/// Writes a message to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// The <paramref name="messageDelegate"/> is only invoked when the message is effectively being sent to the console, so this variant of DebugLog is meant to
		/// be used when constructing the message can be an expensive operation.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="messageDelegate">Function that creates the message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate)
			where TVerbosable : MonoBehaviour, IVerbosable {
			if (verbosable.IsVerbose) Debug.Log(messageDelegate.Invoke(), verbosable);
		}
	}
}