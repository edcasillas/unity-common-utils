using System;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class VerbosableExtensions {
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, string message)
			where TVerbosable : MonoBehaviour, IVerbosable {
			if (verbosable.IsVerbose) Debug.Log(message, verbosable);
		}
		
		public static void DebugLog<TVerbosable>(this TVerbosable verbosable, Func<string> messageDelegate)
			where TVerbosable : MonoBehaviour, IVerbosable {
			if (verbosable.IsVerbose) Debug.Log(messageDelegate.Invoke(), verbosable);
		}
	}
}