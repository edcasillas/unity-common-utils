using System;
using CommonUtils.UnityComponents;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Extensions {
	public static class VerbosableExtensions {
		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, string message)
			where TVerbosable : Object, IVerbosable {
			if (verbosable.IsVerbose) Debug.Log($"[{typeof(TVerbosable).Name}] {message}", verbosable);
		}

		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		/// <remarks>Use this variant when you don't want the log message to be linked to the GameObject context, or
		/// when the verbosable object is not an <see cref="IUnityComponent"/>.</remarks>
		public static void DebugLogNoContext<TVerbosable>(this TVerbosable verbosable, string message)
			where TVerbosable : IVerbosable {
			if (verbosable.IsVerbose) Debug.Log(message);
		}

		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, object message)
			where TVerbosable : Object, IVerbosable {
			if (verbosable.IsVerbose) Debug.Log(message, verbosable);
		}

		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		public static void DebugLog2<TVerbosable>(this TVerbosable verbosable, string message) // TODO Find a way to resolve naming conflicts
			where TVerbosable : IVerbosable, IUnityComponent {
			if (verbosable.IsVerbose) Debug.Log(message, verbosable.gameObject);
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
			where TVerbosable : Object, IVerbosable {
			if (verbosable.IsVerbose) Debug.Log(messageDelegate.Invoke(), verbosable);
		}

		/// <summary>
		/// Writes a message to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// The <paramref name="messageDelegate"/> is only invoked when the message is effectively being sent to the console, so this variant of DebugLog is meant to
		/// be used when constructing the message can be an expensive operation.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="messageDelegate">Function that creates the message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		public static void DebugLog2<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate)
			where TVerbosable : IVerbosable, IUnityComponent {
			if (verbosable.IsVerbose) Debug.Log(messageDelegate.Invoke(), verbosable.gameObject);
		}
	}
}