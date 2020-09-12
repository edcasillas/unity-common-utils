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

		public static Coroutine WaitForSeconds(this MonoBehaviour target, float seconds, Action action) => target.StartCoroutine(waitForSeconds(seconds, action));

		private static IEnumerator waitForSeconds(float seconds, Action action) {
			yield return new WaitForSeconds(seconds);
			action.Invoke();
		}

        public static Coroutine WaitForFixedUpdate(this MonoBehaviour target, Action action) => target.StartCoroutine(waitForFixedUpdate(action));

        private static IEnumerator waitForFixedUpdate(Action action) {
            yield return new WaitForFixedUpdate();
            action.Invoke();
        }

		public static Coroutine WaitForEndOfFrame(this MonoBehaviour target, Action action) => target.StartCoroutine(waitForEndOfFrame(action));

		private static IEnumerator waitForEndOfFrame(Action action) {
			yield return new WaitForEndOfFrame();
			action.Invoke();
		}
	}
}