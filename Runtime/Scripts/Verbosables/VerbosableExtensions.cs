using CommonUtils.UnityComponents;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

// TODO Add XML documentation and relevant help in obsolete attributes.

namespace CommonUtils.Verbosables {
	public static class VerbosableExtensions {
		public static void Log<TVerbosable>(this TVerbosable verbosable, string message, LogLevel logLevel = LogLevel.Debug) where TVerbosable : Object, IVerbosable {
			if (verbosable.Verbosity.HasFlag(Verbosity.Debug)) Debug.Log($"{getVerbosableTagFromMonoBehaviour(verbosable)} {message}", verbosable);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Warning)) Debug.LogWarning($"{getVerbosableTagFromMonoBehaviour(verbosable)} {message}", verbosable);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Error)) Debug.LogError($"{getVerbosableTagFromMonoBehaviour(verbosable)} {message}", verbosable);
			else if (verbosable.Verbosity != Verbosity.None) throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
		}

		public static void Log<TVerbosable>(this TVerbosable verbosable, object message, LogLevel logLevel = LogLevel.Debug) where TVerbosable : Object, IVerbosable {
			if (verbosable.Verbosity.HasFlag(Verbosity.Debug)) Debug.Log($"{getVerbosableTagFromMonoBehaviour(verbosable)} {message}", verbosable);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Warning)) Debug.LogWarning($"{getVerbosableTagFromMonoBehaviour(verbosable)} {message}", verbosable);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Error)) Debug.LogError($"{getVerbosableTagFromMonoBehaviour(verbosable)} {message}", verbosable);
			else if (verbosable.Verbosity != Verbosity.None) throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
		}

		public static void Log<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate, LogLevel logLevel = LogLevel.Debug) where TVerbosable : Object, IVerbosable {
			if (verbosable.Verbosity.HasFlag(Verbosity.Debug)) Debug.Log($"{getVerbosableTagFromMonoBehaviour(verbosable)} {messageDelegate.Invoke()}", verbosable);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Warning)) Debug.LogWarning($"{getVerbosableTagFromMonoBehaviour(verbosable)} {messageDelegate.Invoke()}", verbosable);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Error)) Debug.LogError($"{getVerbosableTagFromMonoBehaviour(verbosable)} {messageDelegate.Invoke()}", verbosable);
			else if (verbosable.Verbosity != Verbosity.None) throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
		}

		public static void Log2<TVerbosable>(this TVerbosable verbosable, string message, LogLevel logLevel = LogLevel.Debug) where TVerbosable : IVerbosable, IUnityComponent {
			if (verbosable.Verbosity.HasFlag(Verbosity.Debug)) Debug.Log($"{getVerbosableTagFromUnityComponent(verbosable)} {message}", verbosable.gameObject);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Warning)) Debug.LogWarning($"{getVerbosableTagFromUnityComponent(verbosable)} {message}", verbosable.gameObject);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Error)) Debug.LogError($"{getVerbosableTagFromUnityComponent(verbosable)} {message}", verbosable.gameObject);
			else if (verbosable.Verbosity != Verbosity.None) throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
		}

		public static void Log2<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate, LogLevel logLevel = LogLevel.Debug) where TVerbosable : IVerbosable, IUnityComponent {
			if (verbosable.Verbosity.HasFlag(Verbosity.Debug)) Debug.Log(messageDelegate.Invoke(), verbosable.gameObject);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Warning)) Debug.LogWarning(messageDelegate.Invoke(), verbosable.gameObject);
			else if (verbosable.Verbosity.HasFlag(Verbosity.Error)) Debug.LogError(messageDelegate.Invoke(), verbosable.gameObject);
			else if (verbosable.Verbosity != Verbosity.None) throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
		}

		public static void LogNoContext<TVerbosable>(this TVerbosable verbosable, string message, LogLevel logLevel = LogLevel.Debug) where TVerbosable : IVerbosable {
			if (verbosable.Verbosity.HasFlag(Verbosity.Debug)) Debug.Log($"[{typeof(TVerbosable).Name}] {message}");
			else if (verbosable.Verbosity.HasFlag(Verbosity.Warning)) Debug.LogWarning($"[{typeof(TVerbosable).Name}] {message}");
			else if (verbosable.Verbosity.HasFlag(Verbosity.Error)) Debug.LogError($"[{typeof(TVerbosable).Name}] {message}");
			else if (verbosable.Verbosity != Verbosity.None) throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
		}

		public static void LogNoContext<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate, LogLevel logLevel = LogLevel.Debug) where TVerbosable : IVerbosable {
			if (verbosable.Verbosity.HasFlag(Verbosity.Debug))Debug.Log($"[{typeof(TVerbosable).Name}] {messageDelegate.Invoke()}");
			else if (verbosable.Verbosity.HasFlag(Verbosity.Warning)) Debug.LogWarning($"[{typeof(TVerbosable).Name}] {messageDelegate.Invoke()}");
			else if (verbosable.Verbosity.HasFlag(Verbosity.Error)) Debug.LogError($"[{typeof(TVerbosable).Name}] {messageDelegate.Invoke()}");
			else if (verbosable.Verbosity != Verbosity.None) throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
		}

		#region Deprecated extensions
		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		[Obsolete("Use the "+ nameof(Log) +" extension method instead.")]
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, string message) where TVerbosable : Object, IVerbosable => verbosable.Log(message);

		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		/// <remarks>Use this variant when you don't want the log message to be linked to the GameObject context, or
		/// when the verbosable object is not an <see cref="IUnityComponent"/>.</remarks>
		[Obsolete("Use the "+ nameof(LogNoContext) +" extension method instead.")]
		public static void DebugLogNoContext<TVerbosable>(this TVerbosable verbosable, string message) where TVerbosable : IVerbosable => verbosable.LogNoContext(message);

		[Obsolete("Use the "+ nameof(LogNoContext) +" extension method instead.")]
		public static void DebugLogNoContext<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate) where TVerbosable : IVerbosable => verbosable.LogNoContext(messageDelegate);

		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		[Obsolete("Use the "+ nameof(Log) +" extension method instead.")]
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, object message) where TVerbosable : Object, IVerbosable => verbosable.Log(message);

		/// <summary>
		/// Writes a <paramref name="message"/> to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		[Obsolete("Use the "+ nameof(Log2) +" extension method instead.")]
		public static void
			DebugLog2<TVerbosable>(this TVerbosable verbosable,
				string message) // TODO Find a way to resolve naming conflicts
			where TVerbosable : IVerbosable, IUnityComponent
			=> verbosable.Log2(message);

		/// <summary>
		/// Writes a message to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// The <paramref name="messageDelegate"/> is only invoked when the message is effectively being sent to the console, so this variant of DebugLog is meant to
		/// be used when constructing the message can be an expensive operation.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="messageDelegate">Function that creates the message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		[Obsolete("Use the "+ nameof(Log) +" extension method instead.")]
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate)
			where TVerbosable : Object, IVerbosable
			=> verbosable.Log(messageDelegate);

		/// <summary>
		/// Writes a message to the console if the <paramref name="verbosable"/> has its <see cref="IVerbosable.IsVerbose"/> property set to true.
		/// The <paramref name="messageDelegate"/> is only invoked when the message is effectively being sent to the console, so this variant of DebugLog is meant to
		/// be used when constructing the message can be an expensive operation.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="messageDelegate">Function that creates the message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		[Obsolete("Use the "+ nameof(Log2) +" extension method instead.")]
		public static void DebugLog2<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate)
			where TVerbosable : IVerbosable, IUnityComponent
			=> verbosable.Log2(messageDelegate);

		/// <summary>
		/// Writes an error <paramref name="message"/> to the console.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		[Obsolete("Use the "+ nameof(Log) +" extension method instead.")]
		public static void LogError<TVerbosable>(this TVerbosable verbosable, string message)
			where TVerbosable : Object, IVerbosable
			=> verbosable.Log(message, LogLevel.Error);

		/// <summary>
		/// Writes an error <paramref name="message"/> to the console.
		/// </summary>
		/// <param name="verbosable">Verbosable component writing the message.</param>
		/// <param name="message">Message to be sent to the console.</param>
		/// <typeparam name="TVerbosable">Type of verbosable component.</typeparam>
		[Obsolete("Use the "+ nameof(LogNoContext) +" extension method instead.")]
		public static void LogErrorNoContext<TVerbosable>(this TVerbosable verbosable, string message)
			where TVerbosable : IVerbosable
			=> verbosable.LogNoContext(message);
		#endregion

		private static string getVerbosableTagFromMonoBehaviour<TVerbosable>(TVerbosable verbosable)
			where TVerbosable : Object, IVerbosable => $"[{typeof(TVerbosable).Name}:{verbosable.name}]";

		private static string getVerbosableTagFromUnityComponent<TVerbosable>(TVerbosable verbosable)
			where TVerbosable : IUnityComponent, IVerbosable => $"[{typeof(TVerbosable).Name}:{verbosable.name}]";
	}
}