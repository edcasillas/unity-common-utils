using CommonUtils.Coroutines;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace CommonUtils.Tests.PlayMode.Coroutines {
	public class CoroutineExtensionsTests {
		private class TestComponent : MonoBehaviour {}

		#region StartCoroutineWithFinishCallback_ExecutesCallbackWhenFinished
		[UnityTest]
		[Timeout(7000)]
		public IEnumerator StartCoroutineWithFinishCallback_ExecutesCallbackWhenFinished() {
			var subject = new GameObject().AddComponent<TestComponent>();

			var coroutineFinished = false;
			var callbackExecuted = false;

			subject.StartCoroutineWithFinishCallback(waitThenExecute(1f,
					() => { coroutineFinished = true; }),
				() => { callbackExecuted = true; });

			yield return new WaitUntil(() => callbackExecuted);

			Assert.IsTrue(callbackExecuted, "Callback was not executed");
			Assert.IsTrue(coroutineFinished, "Coroutine didn't finish before callback");
		}

		private IEnumerator waitThenExecute(float waitSeconds, Action action) {
			yield return new WaitForSeconds(waitSeconds);
			action.Invoke();
		}
		#endregion

		[UnityTest]
		[Timeout(7000)]
		public IEnumerator StartCoroutineWithTimeout_ExecutesOnTimeout() {
			var subject = new GameObject().AddComponent<TestComponent>();

			var finished = false;
			var timedout = false;

			// Execute a coroutine that takes 5 seconds to complete, with a timeout of 1 second. We expect the onTimeout
			// callback to be executed.
			subject.StartCoroutineWithTimeout(
				simpleWaitForSecondsRoutine(5),
				() => {
					finished = true;
					Debug.Log($"Coroutine has been marked as finished.");
				},
				() => {
					timedout = true;
					Debug.Log($"Coroutine has been marked as timed out.");
				},
				1f);

			// Wait until either flag has been set (the Timeout attribute will help in case none of them is actually executed, which would be an error!)
			yield return new WaitUntil(() => finished || timedout);

			if (timedout) {
				Assert.Pass("The coroutine has timed out.");
			} else if(finished) {
				Assert.Fail("The coroutine has finished running, but we were expecting it to timeout.");
			} else {
				Assert.Fail("Nor finished or timedout has been set; the test itself has timedout.");
			}
		}

		private IEnumerator simpleWaitForSecondsRoutine(float seconds) {
			yield return new WaitForSeconds(seconds);
			Debug.Log($"{nameof(simpleWaitForSecondsRoutine)} finished.");
		}
	}
}