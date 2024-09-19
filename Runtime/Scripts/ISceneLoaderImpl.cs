using CommonUtils.Coroutines;
using CommonUtils.Verbosables;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonUtils
{
    public interface ISceneLoaderImpl
    {
		bool IsActive { get; }
		void LoadScene(int sceneIndex);
		void LoadScene(int sceneIndex, Action<AsyncOperation> onReadyToActivate);
		void LoadScene(string scenePath);
		void LoadScene(string scenePath, Action<AsyncOperation> onReadyToActivate);
	}

	internal sealed class SimpleSceneLoader : ISceneLoaderImpl, IVerbosable {
		public bool IsActive => false;
		public Verbosity Verbosity => Verbosity.Warning;

		public void LoadScene(int sceneIndex) => SceneManager.LoadScene(sceneIndex);
		public void LoadScene(int sceneIndex, Action<AsyncOperation> onReadyToActivate) => Coroutiner.StartCoroutine(doLoad(sceneIndex, onReadyToActivate));
		public void LoadScene(string scenePath) => SceneManager.LoadScene(scenePath);
		public void LoadScene(string scenePath, Action<AsyncOperation> onReadyToActivate) => Coroutiner.StartCoroutine(doLoad(scenePath, onReadyToActivate));

		private IEnumerator doLoad(int sceneIndex, Action<AsyncOperation> onReadyToActivate) {
			var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
			if (asyncLoad == null) {
				this.LogNoContext($"Could not load scene with build index {sceneIndex}. SceneManager.LoadSceneAsync failed. Aborting.");
				yield break;
			}
			asyncLoad.allowSceneActivation = false;
			while(asyncLoad.progress < 0.9f) {
				yield return null;
			}
			onReadyToActivate(asyncLoad);
		}

		private IEnumerator doLoad(string scenePath, Action<AsyncOperation> onReadyToActivate) {
			var asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Single);
			if (asyncLoad == null) {
				this.LogNoContext($"Could not load scene with path \"{scenePath}\". SceneManager.LoadSceneAsync failed. Aborting.");
				yield break;
			}
			asyncLoad.allowSceneActivation = false;
			while(asyncLoad.progress < 0.9f) {
				yield return null;
			}
			onReadyToActivate(asyncLoad);
		}
	}
}
