using System;
using System.Collections;
using UnityEngine;

namespace CommonUtils.Coroutines {
	public static class CoroutineExtensions {
		public static Coroutine StartCoroutineWithFinishCallback(this MonoBehaviour monoBehaviour, IEnumerator coroutine, Action callback)
			=> monoBehaviour.StartCoroutine(executeCoroutineWithFinishCallback(monoBehaviour, coroutine, callback));

		private static IEnumerator executeCoroutineWithFinishCallback(MonoBehaviour monoBehaviour, IEnumerator coroutine, Action callback) {
			yield return monoBehaviour.StartCoroutine(coroutine);
			callback.Invoke();
		}
	}
}
