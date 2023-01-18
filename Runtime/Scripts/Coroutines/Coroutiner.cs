using CommonUtils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Coroutines {
	/// <summary>
	/// Creates a <see cref="MonoBehaviour"/> instance through which static classes can call <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>.
	/// </summary>
	/// <author>Sebastiaan Fehr (Seb@TheBinaryMill.com)</author>
	/// <description>
	/// Classes that do not inherit from MonoBehaviour, or static methods within MonoBehaviours are inertly unable to
	/// call StartCoroutine, as this method is not static and does not exist on Object.This Class creates a proxy
	/// through which StartCoroutine can be called.
	/// </description>
	/// <modified_by>Ed Casillas</modified_by>
	public static class Coroutiner {
		#region Pools
		private static readonly Queue<CoroutinerInstance> instancePool = new Queue<CoroutinerInstance>();

		/// <summary>
		/// A pool of instances marked as DontDestroyOnLoad. This must be different than the regular instance pool
		/// because we can't revert a DontDestroyOnLoad, and we don't want our coroutines to keep running between scenes
		/// unless explicitly specified during the call to <see cref="StartCoroutine"/>.
		/// </summary>
		private static readonly Queue<CoroutinerInstance> persistentInstancePool = new Queue<CoroutinerInstance>();
		#endregion

		/// <summary>
		/// Creates a temporary <see cref="GameObject"/> to handle a <see cref="Coroutine"/>; this GameObject is destroyed when coroutine is finished.
		/// </summary>
		/// <returns>The <see cref="CoroutinerInstance"/> component running the coroutine.</returns>
		/// <param name="coroutine">Coroutine to run.</param>
		/// <param name="gameObjectName">Name of the GameObject that'll run the coroutine.</param>
		/// <param name="preventDestroyOnSceneChange">Should this coroutine continue running during a scene change?</param>
		public static CoroutinerInstance StartCoroutine(IEnumerator coroutine, string gameObjectName = "Active Coroutiner", bool preventDestroyOnSceneChange = false) {
			if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

			var routineHandler = getInstance(preventDestroyOnSceneChange);
			routineHandler.name = gameObjectName;

			// Actually start the coroutine
			routineHandler.ProcessWork(coroutine);
			// Return the CoroutinerInstance handling the coroutine.
			return routineHandler;
		}

		private static CoroutinerInstance getInstance(bool dontDestroyOnLoad) {
			CoroutinerInstance result = null;

			/*
			 * NOTE Even if we're able to retrieve an instance value from the pool, it doesn't mean that instance is
			 * valid; it might be the case that the instance has been destroyed, thus !result will return false; this
			 * is why we iterate until we have a valid instance.
			 */
			while (!result) {
				switch (dontDestroyOnLoad) {
					case true when persistentInstancePool.Any():
						result = persistentInstancePool.Dequeue();
						break;
					case false when instancePool.Any():
						result = instancePool.Dequeue();
						break;
					default: {
						// Create empty GameObject to handle task.
						var routineHandler = new GameObject();

						// Attach script to run coroutines
						result = routineHandler.AddComponent<CoroutinerInstance>();
						if (dontDestroyOnLoad) {
							Object.DontDestroyOnLoad(result);
							result.InstancePool = persistentInstancePool;
						} else {
							result.InstancePool = instancePool;
						}

						break;
					}
				}
			}

			return result;
		}

		#region WaitForFrames
		public static CoroutinerInstance WaitForFrames(Action action, int framesToWait = 1,
			string gameObjectName = "Active Coroutiner", bool preventDestroyOnSceneChange = false) => StartCoroutine(
			waitUntilNextFrame(action, framesToWait),
			gameObjectName,
			preventDestroyOnSceneChange);

		private static IEnumerator waitUntilNextFrame(Action action, int framesToWait) {
			while (framesToWait > 0) {
				yield return null;
				framesToWait--;
			}
			action.Invoke();
		}
		#endregion
	}

	/// <summary>
	/// GameObject component to handle coroutines.
	/// </summary>
	public class CoroutinerInstance : EnhancedMonoBehaviour {
		/// <summary>
		/// The running coroutine.
		/// </summary>
		private readonly IList<Coroutine> runningCoroutines = new List<Coroutine>();

		/// <summary>
		/// Used when running multiple coroutines to keep track of which of them already finished.
		/// A float is used instead of a simple bool because in the near future we want to integrate more
		/// granular progress (10%, 50%, etc)
		/// </summary>
		private readonly IList<float> progress;

		public Queue<CoroutinerInstance> InstancePool { get; set; }

		[ShowInInspector] public int RunningCoroutinesCount => runningCoroutines.Count;

		[ShowInInspector] public float OverallProgress => progress.Any() ? progress.Sum() / progress.Count() : 0f;

		/// <summary>
		/// Processes the work.
		/// </summary>
		/// <returns>Running coroutine.</returns>
		/// <param name="coroutine">Coroutine method to run.</param>
		internal Coroutine ProcessWork(IEnumerator coroutine) {
			if (runningCoroutines.Any()) {
				throw new InvalidOperationException(
					$"{runningCoroutines.Count} coroutines are still running in this instance.");
			}

			var result = this.StartCoroutineWithFinishCallback(coroutine, OnFinished);
			runningCoroutines.Add(result);
			return result;
		}

		#region ProcessWork with multiple coroutines
		internal void ProcessWork(ICollection<IEnumerator> coroutines, Action onFinishedAll, Action<float> onProgress = null) {
			#region Input validation
			if (runningCoroutines.Any()) {
				throw new InvalidOperationException(
					$"{runningCoroutines.Count} coroutines are still running in this instance.");
			}
			if (coroutines.IsNullOrEmpty()) {
				throw new ArgumentException("At least one coroutine is required.", nameof(coroutines));
			}
			if (onFinishedAll == null) throw new ArgumentNullException(nameof(onFinishedAll));
			#endregion

			for (var i = 0; i < coroutines.Count(); i++) {
				progress.Add(0);
				StartCoroutine(executeCoroutine(coroutines.ElementAt(0), i));
			}

			StartCoroutine(waitForAllFinished(onFinishedAll, onProgress));
		}

		private IEnumerator executeCoroutine(IEnumerator coroutine, int progressIndex) {
			yield return coroutine;
			progress[progressIndex] = 1;
		}

		private IEnumerator waitForAllFinished(Action onFinishedAll, Action<float> onProgress) {
			var prevProgress = 0f;
			do {
				if(prevProgress >= OverallProgress) continue;
				onProgress?.Invoke(OverallProgress);
				prevProgress = OverallProgress;
				yield return null;
			} while (OverallProgress < 1);

			onProgress?.Invoke(1f);
			onFinishedAll.Invoke();
		}
		#endregion

		/// <summary>
		/// Stops the coroutine and destroys the GameObject running the coroutine.
		/// </summary>
		public void StopAndDestroy() {
			if (runningCoroutines.Any()) {
				foreach (var runningCoroutine in runningCoroutines) {
					StopCoroutine(runningCoroutine);
				}
			}

			OnFinished();
		}

		private void OnFinished() {
			runningCoroutines.Clear();
			progress.Clear();
			if(InstancePool == null) Destroy(gameObject);
			else {
				name = "Idle Coroutiner";
				InstancePool.Enqueue(this);
			}
		}
	}
}