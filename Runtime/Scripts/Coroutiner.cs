using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils {
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
		private static readonly Queue<CoroutinerInstance> instancePool = new Queue<CoroutinerInstance>();

		/// <summary>
		/// Creates a temporary <see cref="GameObject"/> to handle a <see cref="Coroutine"/>; this GameObject is destroyed when coroutine is finished.
		/// </summary>
		/// <returns>The CoroutinerInstance component running the coroutine.</returns>
		/// <param name="coroutine">Coroutine to run.</param>
		/// <param name="gameObjectName">Name of the GameObject that'll run the coroutine.</param>
		public static CoroutinerInstance StartCoroutine(IEnumerator coroutine, string gameObjectName = "Active Coroutiner", bool preventDestroyOnSceneChange = false) {
			CoroutinerInstance routineHandler = null;

			while (!routineHandler) {
				if (instancePool.Any()) routineHandler = instancePool.Dequeue();
				else {
					// Create empty GameObject to handle task.
					var routeneHandlerGo = new GameObject(gameObjectName);

					// Attach script to run coroutines
					routineHandler              = routeneHandlerGo.AddComponent<CoroutinerInstance>();
					routineHandler.InstancePool = instancePool;
				}
			}

			// Actually start the coroutine
			routineHandler.ProcessWork(coroutine, preventDestroyOnSceneChange);
			// Return the CoroutinerInstance handling the coroutine.
			return routineHandler;
		}

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
	}

	/// <summary>
	/// GameObject component to handle coroutines.
	/// </summary>
	public class CoroutinerInstance : MonoBehaviour {
		/// <summary>
		/// The running coroutine.
		/// </summary>
		private Coroutine runningCoroutine;

		public Queue<CoroutinerInstance> InstancePool { get; set; }

		/// <summary>
		/// Processes the work.
		/// </summary>
		/// <returns>Running coroutine.</returns>
		/// <param name="coroutine">Coroutine method to run.</param>
		public Coroutine ProcessWork(IEnumerator coroutine, bool preventDestroyOnSceneChange)
		{
			if(preventDestroyOnSceneChange) DontDestroyOnLoad(this);
			runningCoroutine = StartCoroutine(DestroyWhenComplete(coroutine));
			return runningCoroutine;
		}

		/// <summary>
		/// Stops the coroutine and destroys the GameObject running the coroutine.
		/// </summary>
		public void StopAndDestroy() {
			if (runningCoroutine != null) {
				StopCoroutine(runningCoroutine);
			}

			OnFinished();
		}

		/// <summary>
		/// Destroy the game object when the coroutine has finished.
		/// </summary>
		/// <returns>The when complete.</returns>
		/// <param name="iterationResult">Iteration result.</param>
		public IEnumerator DestroyWhenComplete(IEnumerator iterationResult) {
			yield return StartCoroutine(iterationResult);
			OnFinished();
		}

		private void OnFinished() {
			if(InstancePool == null) Destroy(gameObject);
			else {
				name = "Idle Coroutiner";
				InstancePool.Enqueue(this);
			}
		}
	}
}