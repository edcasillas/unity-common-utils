using CommonUtils.Coroutines;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace CommonUtils.Tests.PlayMode.Coroutines {
	public class CoroutinerTests {
		[UnityTest]
		[Timeout(7000)]
		public IEnumerator StartCoroutines_Pass() {
			var coroutines = new List<IEnumerator>();
			for (var i = 0; i < 10; i++) {
				coroutines.Add(waitRandom());
			}
			Debug.Log($"{coroutines.Count} coroutines will be executed in this test.");

			var allFinished = false;

			var coroutiner = Coroutiner.StartCoroutines(coroutines, () => { allFinished = true; }, verbose: true);

			Debug.Log("Waiting for all coroutines to be finished.");

			var timeout = 7f;
			while (!allFinished && timeout > 0) {
				timeout -= Time.deltaTime;
				yield return null;
			}

			if (timeout <= 0) {
				Debug.Log("Test timed out");
			}

			Debug.Log($"WaitUntil has finished. Coroutiner reports progress: {coroutiner}");

			Assert.IsTrue(allFinished);
		}

		private static IEnumerator waitRandom() {
		    var waitTime = Random.Range(0.1f, 2f);
			Debug.Log($"Starting waiting random time: {waitTime}.");
			yield return new WaitForSeconds(Random.Range(0.1f, 2f));
			Debug.Log($"Finished waiting for {waitTime} seconds.");
		}
	}
}