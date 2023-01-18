using CommonUtils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.Coroutines {
	public static class CoroutineOrchestrator {


		public static void StartCoroutines(ICollection<IEnumerator> coroutines, Action onFinishedAll, Action<float> onProgress = null) {
			if (coroutines.IsNullOrEmpty()) {
				throw new ArgumentException("At least one coroutine is required.", nameof(coroutines));
			}
			if (onFinishedAll == null) throw new ArgumentNullException(nameof(onFinishedAll));

			var progress = new List<float>();

			for (var i = 0; i < coroutines.Count(); i++) {
				progress.Add(0);
				Coroutiner.StartCoroutine(executeCoroutine(coroutines.ElementAt(0), progress, i));
			}

			Coroutiner.StartCoroutine(waitForAllFinished(progress, onFinishedAll, onProgress));
		}

		private static IEnumerator executeCoroutine(IEnumerator coroutine, IList<float> progress, int progressIndex) {
			yield return coroutine;
			progress[progressIndex] = 1;
		}

		private static IEnumerator waitForAllFinished(IList<float> progress, Action onFinishedAll, Action<float> onProgress) {
			var overallProgress = 0f;
			do {
				overallProgress = progress.Sum() / progress.Count();
				onProgress?.Invoke(overallProgress);
				yield return null;
			} while (overallProgress < 1);

			onProgress?.Invoke(1f);
			onFinishedAll.Invoke();
		}
	}
}
