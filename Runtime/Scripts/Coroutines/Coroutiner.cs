using CommonUtils.Extensions;
using CommonUtils.Verbosables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Coroutines {
	/// <summary>
	/// Contains methods to execute coroutines that can be called from any class, either deriving from <see cref="MonoBehaviour"/> or not.
	/// </summary>
	/// <author>Ed Casillas</author>
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
		/// Creates a temporary <see cref="GameObject"/> to handle a <see cref="Coroutine"/>.
		/// This method allows classes not deriving from <see cref="MonoBehaviour"/> to execute coroutines (e.g. static classes).
		/// </summary>
		/// <returns>The <see cref="CoroutinerInstance"/> component running the coroutine.</returns>
		/// <param name="coroutine">Coroutine to run.</param>
		/// <param name="gameObjectName">Name of the GameObject that'll run the coroutine.</param>
		/// <param name="preventDestroyOnSceneChange">Should this coroutine continue running during a scene change?</param>
		/// <param name="verbosity">Should this coroutiner instance log messages to the console?</param>
		/// <remarks>
		/// Classes that do not inherit from MonoBehaviour, or static methods within MonoBehaviours are inertly unable to
		/// call StartCoroutine, as this method is not static and does not exist on Object.This Class creates a proxy
		/// through which StartCoroutine can be called.
		///
		/// Original credit to Sebastiaan Fehr (Seb@TheBinaryMill.com)
		/// </remarks>
		public static CoroutinerInstance StartCoroutine(IEnumerator coroutine, string gameObjectName = "Active Coroutiner", bool preventDestroyOnSceneChange = false, Verbosity verbosity = Verbosity.None) {
			if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

			var routineHandler = getInstance(gameObjectName, preventDestroyOnSceneChange, verbosity);

			// Actually start the coroutine
			routineHandler.ProcessWork(coroutine);
			// Return the CoroutinerInstance handling the coroutine.
			return routineHandler;
		}

		/// <summary>
		/// Starts a collection of <paramref name="coroutines"/> and defines callbacks to inform the caller as they
		/// become completed.
		/// </summary>
		/// <param name="coroutines">Collection of coroutines to be executed.</param>
		/// <param name="onFinishedAll">Callback to be executed when all coroutines are finished.</param>
		/// <param name="onProgress">Callback to be executed when some progress has been done executing the coroutines.</param>
		/// <param name="gameObjectName">Name of the GameObject that'll run the coroutines.</param>
		/// <param name="preventDestroyOnSceneChange">Should these coroutines continue running during a scene change?</param>
		/// <param name="verbosity">Should this coroutiner instance log messages to the console?</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static CoroutinerInstance StartCoroutines(ICollection<IEnumerator> coroutines, Action onFinishedAll, Action<float> onProgress = null, string gameObjectName = "Active Coroutiner", bool preventDestroyOnSceneChange = false, Verbosity verbosity = Verbosity.None) {
			if (coroutines.IsNullOrEmpty()) {
				throw new ArgumentException($"{nameof(coroutines)} parameter cannot be null or empty.",
					nameof(coroutines));
			}

			var routineHandler = getInstance(gameObjectName, preventDestroyOnSceneChange, verbosity);

			// Actually start the coroutines
			routineHandler.ProcessWork(coroutines, onFinishedAll, onProgress);
			// Return the CoroutinerInstance handling the coroutine.
			return routineHandler;
		}

		private static CoroutinerInstance getInstance(string gameObjectName, bool dontDestroyOnLoad, Verbosity verbosity) {
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
						if(verbosity.HasFlag(Verbosity.Debug)) Debug.Log("A coroutine instance has been fetched from the pool.");
						break;
					case false when instancePool.Any():
						result = instancePool.Dequeue();
						if(verbosity.HasFlag(Verbosity.Debug)) Debug.Log("A coroutine instance has been fetched from the pool.");
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

						if(verbosity.HasFlag(Verbosity.Debug)) Debug.Log("A new coroutiner instance has been created.");

						break;
					}
				}
			}

			result.gameObject.name = gameObjectName;
			result.SetVerbosity(verbosity);

			return result;
		}
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
		private readonly IList<float> progress = new List<float>();

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

			this.DebugLog("Starting coroutine");
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

			this.DebugLog(() => $"Starting {coroutines.Count} coroutines.");
			for (var i = 0; i < coroutines.Count(); i++) {
				progress.Add(0);
				this.DebugLog($"Starting coroutine {i}");
				StartCoroutine(executeCoroutine(coroutines.ElementAt(i), i));
			}

			this.DebugLog($"Starting coroutine to wait for all finished.");
			StartCoroutine(waitForAllFinished(onFinishedAll, onProgress));
		}

		private IEnumerator executeCoroutine(IEnumerator coroutine, int progressIndex) {
			yield return coroutine;
			this.DebugLog(() => $"Coroutine at index {progressIndex} has finished executing.");
			progress[progressIndex] = 1;
		}

		private IEnumerator waitForAllFinished(Action onFinishedAll, Action<float> onProgress) {
			var prevProgress = 0f;
			do {
				yield return null;
				if (prevProgress >= OverallProgress) continue;
				this.DebugLog(() => $"Progress: {OverallProgress * 100}%");
				onProgress?.Invoke(OverallProgress);
				prevProgress = OverallProgress;
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

			this.DebugLog(() => $"Requested to stop {runningCoroutines.Count} running coroutines.");

			OnFinished();
		}

		internal void SetVerbosity(Verbosity verbosity) => Verbosity = verbosity;

		private void OnFinished() {
			this.DebugLog("All coroutines have finished executing.");
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