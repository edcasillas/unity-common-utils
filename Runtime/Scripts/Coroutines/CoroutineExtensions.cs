using System;
using System.Collections;
using UnityEngine;

namespace CommonUtils.Coroutines {
	public static class CoroutineExtensions {
		#region StartCoroutineWithTimeout
		public static Coroutine StartCoroutineWithTimeout(this MonoBehaviour monoBehaviour, IEnumerator coroutine, Action onFinished, Action onTimeout, float timeout) {
			var isFinished = false;
			var runningCoroutine = monoBehaviour.StartCoroutineWithFinishCallback(coroutine, () => { isFinished = true; });
			monoBehaviour.WaitUntil(() => isFinished, onFinished,()=> {
				monoBehaviour.StopCoroutine(runningCoroutine);
				onTimeout?.Invoke();
			}, timeout);

			return runningCoroutine;
		}

		#endregion

		#region StartCoroutineWithFinishCallback
		public static Coroutine StartCoroutineWithFinishCallback(this MonoBehaviour monoBehaviour, IEnumerator coroutine, Action callback)
			=> monoBehaviour.StartCoroutine(executeCoroutineWithFinishCallback(monoBehaviour, coroutine, callback));

		private static IEnumerator executeCoroutineWithFinishCallback(MonoBehaviour monoBehaviour, IEnumerator coroutine, Action callback) {
			yield return monoBehaviour.StartCoroutine(coroutine);
			callback.Invoke();
		}
		#endregion

		#region WaitForFrames
		public static Coroutine WaitForFrames(this MonoBehaviour monoBehaviour, Action action, int framesToWait = 1)
			=> monoBehaviour.StartCoroutine(
			waitUntilNextFrame(action, framesToWait));

		private static IEnumerator waitUntilNextFrame(Action action, int framesToWait) {
			while (framesToWait > 0) {
				yield return null;
				framesToWait--;
			}
			action.Invoke();
		}
		#endregion

		#region WaitForSeconds
		public static Coroutine WaitForSeconds(this MonoBehaviour monoBehaviour, Action action, float secondsToWait = 1)
			=> monoBehaviour.StartCoroutine(
				WaitForSeconds(action, secondsToWait));

		internal static IEnumerator WaitForSeconds(Action action, float secondsToWait) {
			yield return new WaitForSeconds(secondsToWait);
			action.Invoke();
		}
		#endregion

		#region WaitUntil
		public static Coroutine WaitUntil(this MonoBehaviour monoBehaviour, Func<bool> condition, Action then, Action onTimeout = null, float? timeout = null) {
			if (!monoBehaviour) throw new ArgumentNullException(nameof(monoBehaviour));
			if (condition == null) throw new ArgumentNullException(nameof(condition));
			if (then == null) throw new ArgumentNullException(nameof(then));
			if ((onTimeout != null && !timeout.HasValue) || (onTimeout == null && timeout.HasValue))
				throw new ArgumentException(
					"In order to specify a timeout, please include both timeout and OnTimeout parameters.");

			return monoBehaviour.StartCoroutine(doWaitUntil(condition, then, timeout));
		}

		private static IEnumerator doWaitUntil(Func<bool> condition, Action then, float? timeout = null) {
			while (!condition.Invoke() && timeout is null or > 0) {
				yield return null;
				timeout -= Time.deltaTime;
			}
			then.Invoke();
		}
		#endregion
	}
}
