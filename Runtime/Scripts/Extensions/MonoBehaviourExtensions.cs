using System;
using System.Collections;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class MonoBehaviourExtensions {
		public static Coroutine InvokeNextFrame(this MonoBehaviour target, Action action) => target.StartCoroutine(invokeNextFrame(action));

		private static IEnumerator invokeNextFrame(Action action) {
			yield return new WaitForEndOfFrame();
			action.Invoke();
		}
	}
}