using CommonUtils.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using CommonUtils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
 * TODO
 * - Abort if requested scene is the same as the active one, and an option to opt-out from this (so scenes CAN be reloaded).
 */

namespace CommonUtils {
	[AddComponentMenu("UI/Scene Loader")]
	public class SceneLoader : EnhancedMonoBehaviour {
		private const int MAX_PREVIOUS_SCENES = 5;

		#region Static access members
		private static readonly Stack<int> previousScenes = new Stack<int>();

		private static SceneLoader _instance;
		private static SceneLoader instance {
			get {
				if (!_instance) {
					_instance = FindObjectOfType<SceneLoader>();
					if (!_instance) {
						_instance = Resources.Load<SceneLoader>(nameof(SceneLoader));
					}
				}

				return _instance;
			}
		}

		public static bool IsActive => _instance && _instance.isActiveAndEnabled;

		public static void LoadScene(int sceneIndex) {
			if (!instance) {
				SceneManager.LoadScene(sceneIndex);
				return;
			}
			instance.Load(sceneIndex);
		}

		public static void LoadScene(int sceneIndex, Action<AsyncOperation> onReadyToActivate) {
			if (!instance) {
				Coroutiner.StartCoroutine(loadWithoutInstance(sceneIndex, onReadyToActivate));
				return;
			}
			instance.Load(sceneIndex, onReadyToActivate);
		}

		public static void LoadScene(string scenePath) {
			if (!instance) {
				SceneManager.LoadScene(scenePath);
				return;
			}
			instance.Load(scenePath);
		}

		public static void LoadScene(string scenePath, Action<AsyncOperation> onReadyToActivate) {
			if (!instance) {
				Coroutiner.StartCoroutine(loadWithoutInstance(scenePath, onReadyToActivate));
				return;
			}
			instance.Load(scenePath, onReadyToActivate);
		}

		private static IEnumerator loadWithoutInstance(int sceneIndex, Action<AsyncOperation> onReadyToActivate) {
			var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
			asyncLoad.allowSceneActivation = false;
			while(asyncLoad.progress < 0.9f) {
				yield return null;
			}
			onReadyToActivate(asyncLoad);
		}

		private static IEnumerator loadWithoutInstance(string scenePath, Action<AsyncOperation> onReadyToActivate) {
			var asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Single);
			asyncLoad.allowSceneActivation = false;
			while(asyncLoad.progress < 0.9f) {
				yield return null;
			}
			onReadyToActivate(asyncLoad);
		}

		private static void pushCurrentScene() {
			previousScenes.Push(SceneManager.GetActiveScene().buildIndex);
			#region Trim previous scenes stack when MAX_PREVIOUS_SCENES is reached
			try {
				if (previousScenes.Count > MAX_PREVIOUS_SCENES) {
					var tmpQueue = new Stack<int>();
					for (int i = 0; i < MAX_PREVIOUS_SCENES; i++) {
						tmpQueue.Push(previousScenes.Pop());
					}
					previousScenes.Clear();
					while (tmpQueue.Count > 0) {
						previousScenes.Push(tmpQueue.Pop());
					}
				}
			} catch (Exception e) {
				Debug.LogError($"There's been an error while triming the previousScenes stack: {e.Message}");
				previousScenes.Clear();
			}
			#endregion
		}
		#endregion

		#region Inspector fields
#pragma warning disable 649

		[SerializeField] private TMP_Text suggestionsLabel;

		[SerializeField] private Slider progressSlider;

		[FormerlySerializedAs("Suggestions")]
		[SerializeField] private string[] suggestions;

		[FormerlySerializedAs("WebOnlySuggestions")]
		[SerializeField] private string[] webOnlySuggestions;

		[FormerlySerializedAs("AndroidOnlySuggestions")]
		[SerializeField] private string[] androidOnlySuggestions;

		[FormerlySerializedAs("IOSOnlySuggestions")]
		[SerializeField] private string[] iOSOnlySuggestions;

		[FormerlySerializedAs("MobileOnlySuggestions")]
		[SerializeField] private string[] mobileOnlySuggestions;

		[FormerlySerializedAs("SuggestionsChangeEvery")]
		[SerializeField] [Range(1f, 5f)] private float suggestionsChangeEvery = 1f;
#pragma warning restore 649
		#endregion

		private CoroutinerInstance suggestionsCoroutine;
		private List<string> suggestionsToShow;

		#region Unity Lifecycle
		private void Awake() {
			if (_instance && _instance != this) {
				Destroy(gameObject);
				return;
			}

			_instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += onSceneLoaded;
		}

		private void Start() {
			if(suggestionsLabel) initSuggestions();
		}

		private void OnDestroy() {
			if(suggestionsCoroutine) {
				suggestionsCoroutine.StopAndDestroy();
				suggestionsCoroutine = null;
			}

			SceneManager.sceneLoaded -= onSceneLoaded;
			if (_instance == this) _instance = null;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Loads the scene with the specified <paramref name="sceneIndex"/> and activates it as soon as it's available.
		/// </summary>
		/// <param name="sceneIndex">Build index of the scene to load.</param>
		public void Load(int sceneIndex) {
			gameObject.SetActive(true);
			if(suggestionsLabel) suggestionsCoroutine = Coroutiner.StartCoroutine(updateSuggestions(), "Loading suggestions", true);
			pushCurrentScene();
			Coroutiner.StartCoroutine(doLoad(sceneIndex), "Loading scene");
		}

		/// <summary>
		/// Loads the scene with the specified <paramref name="sceneIndex"/> and executes the <paramref name="onReadyToActivate"/> callback when it's fully loaded and ready to activate.
		/// </summary>
		/// <param name="sceneIndex">Build index of the scene to load.</param>
		/// <param name="onReadyToActivate">Callback to execute when the scene is ready to be activated.</param>
		public void Load(int sceneIndex, Action<AsyncOperation> onReadyToActivate) {
			gameObject.SetActive(true);
			if(suggestionsLabel) suggestionsCoroutine = Coroutiner.StartCoroutine(updateSuggestions(), "Loading suggestions", true);
			pushCurrentScene();
			Coroutiner.StartCoroutine(doLoad(sceneIndex, onReadyToActivate), "Loading scene");
		}

		/// <summary>
		/// Loads the scene with the specified <paramref name="scenePath"/> and activates it as soon as it's available.
		/// </summary>
		/// <param name="scenePath">Path of the scene to load.</param>
		public void Load(string scenePath) {
			gameObject.SetActive(true);
			if(suggestionsLabel) suggestionsCoroutine = Coroutiner.StartCoroutine(updateSuggestions(), "Loading suggestions", true);
			pushCurrentScene();
			Coroutiner.StartCoroutine(doLoad(scenePath), "Loading scene");
		}

		/// <summary>
		/// Loads the scene with the specified <paramref name="scenePath"/> and executes the <paramref name="onReadyToActivate"/> callback when it's fully loaded and ready to activate.
		/// </summary>
		/// <param name="scenePath">Path of the scene to load.</param>
		/// <param name="onReadyToActivate">Callback to execute when the scene is ready to be activated.</param>
		public void Load(string scenePath, Action<AsyncOperation> onReadyToActivate) {
			gameObject.SetActive(true);
			if(suggestionsLabel) suggestionsCoroutine = Coroutiner.StartCoroutine(updateSuggestions(), "Loading suggestions", true);
			pushCurrentScene();
			Coroutiner.StartCoroutine(doLoad(scenePath, onReadyToActivate), "Loading scene");
		}

		public void LoadPrevious() {
			if (previousScenes.Count == 0) return;
			LoadScene(previousScenes.Pop());
		}

		#region doLoad coroutines
		private IEnumerator doLoad(int sceneIndex) {
			var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
			while(!asyncLoad.isDone) {
				if(progressSlider) progressSlider.value = asyncLoad.progress;
				yield return null;
			}
		}

		private IEnumerator doLoad(int sceneIndex, Action<AsyncOperation> onReadyToActivate) {
			var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
			asyncLoad.allowSceneActivation = false;
			while(asyncLoad.progress < 0.9f) {
				if(progressSlider) progressSlider.value = asyncLoad.progress;
				yield return null;
			}

			if(progressSlider) progressSlider.value = 1f;
			onReadyToActivate(asyncLoad);
		}

		private IEnumerator doLoad(string scenePath) {
			var asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Single);
			while(!asyncLoad.isDone) {
				if(progressSlider) progressSlider.value = asyncLoad.progress;
				yield return null;
			}
		}

		private IEnumerator doLoad(string scenePath, Action<AsyncOperation> onReadyToActivate) {
			var asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Single);
			asyncLoad.allowSceneActivation = false;
			while(asyncLoad.progress < 0.9f) {
				if(progressSlider) progressSlider.value = asyncLoad.progress;
				yield return null;
			}

			progressSlider.value = 1f;
			onReadyToActivate(asyncLoad);
		}
		#endregion

		private IEnumerator updateSuggestions() {
			if(suggestionsToShow == null) {
				initSuggestions();
			}
			if(suggestionsLabel && suggestionsToShow != null && suggestionsToShow.Count > 0) {
				while (true) {
					suggestionsLabel.text = suggestionsToShow[Random.Range(0, suggestionsToShow.Count - 1)];
					yield return new WaitForSeconds(suggestionsChangeEvery);
				}
			}

			this.DebugLog("There are no suggestions to show.");
			if (suggestionsLabel) suggestionsLabel.text = string.Empty;
		}

		private void initSuggestions() {
			suggestionsToShow = new List<string>();
			suggestionsToShow.AddRange(suggestions);

			#if UNITY_WEBGL
			suggestionsToShow.AddRange(webOnlySuggestions);
			#endif

			#if UNITY_ANDROID || UNITY_IOS
			suggestionsToShow.AddRange(mobileOnlySuggestions);
			#endif

			#if UNITY_ANDROID
			suggestionsToShow.AddRange(androidOnlySuggestions);
			#endif

			#if UNITY_IOS
			suggestionsToShow.AddRange(iOSOnlySuggestions);
			#endif

			#if UNITY_EDITOR
			suggestionsToShow.Add("You're playing in the editor! :D");
			#endif
		}

		private void onSceneLoaded(Scene scene, LoadSceneMode mode) {
			if(suggestionsCoroutine) {
				suggestionsCoroutine.StopAndDestroy();
				suggestionsCoroutine = null;
			}
			gameObject.SetActive(false);
		}
		#endregion
	}
}