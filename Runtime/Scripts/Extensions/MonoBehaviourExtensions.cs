using System;
using System.Collections;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class MonoBehaviourExtensions {
		public static Coroutine InvokeAfterFrames(this MonoBehaviour target, Action action, int framesToWait) => target.StartCoroutine(invokeAfterFrames(action, framesToWait));

		private static IEnumerator invokeAfterFrames(Action action, int framesToWait) {
			while(framesToWait-- > 0) yield return null;
			action.Invoke();
		}

		public static Coroutine WaitUntil(this MonoBehaviour target, Func<bool> condition, Action action) => target.StartCoroutine(waitUntil(condition, action));

		private static IEnumerator waitUntil(Func<bool> condition, Action action) {
			yield return new WaitUntil(condition);
			action.Invoke();
		}
	}
}