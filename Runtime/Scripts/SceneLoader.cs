using System;
using System.Collections;
using System.Collections.Generic;
using CommonUtils.Extensions;
using CommonUtils.Inspector.ReorderableInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CommonUtils {
	[AddComponentMenu("UI/Scene Loader")]
	public class SceneLoader : MonoBehaviour, IVerbosable {
		#region Static access members
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
			instance.load(sceneIndex);
		}

		public static void LoadScene(int sceneIndex, Action<AsyncOperation> onReadyToActivate) {
			if (!instance) {
				Coroutiner.StartCoroutine(loadWithoutInstance(sceneIndex, onReadyToActivate));
				return;
			}
			instance.load(sceneIndex, onReadyToActivate);
		}

		public static void LoadScene(string scenePath, Action<AsyncOperation> onReadyToActivate) {
			if (!instance) {
				Coroutiner.StartCoroutine(loadWithoutInstance(scenePath, onReadyToActivate));
				return;
			}
			instance.load(scenePath, onReadyToActivate);
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
		#endregion

		#region Inspector fields
#pragma warning disable 649
		[FormerlySerializedAs("SuggestionsLabel")]
		[SerializeField] private Text suggestionsLabel;

		[SerializeField] private Slider progressSlider;

		[FormerlySerializedAs("Suggestions")]
		[SerializeField] [Reorderable] private string[] suggestions;

		[FormerlySerializedAs("WebOnlySuggestions")]
		[SerializeField] [Reorderable] private string[] webOnlySuggestions;

		[FormerlySerializedAs("AndroidOnlySuggestions")]
		[SerializeField] [Reorderable] private string[] androidOnlySuggestions;

		[FormerlySerializedAs("IOSOnlySuggestions")]
		[SerializeField] [Reorderable] private string[] iOSOnlySuggestions;

		[FormerlySerializedAs("MobileOnlySuggestions")]
		[SerializeField] [Reorderable] private string[] mobileOnlySuggestions;

		[FormerlySerializedAs("SuggestionsChangeEvery")]
		[SerializeField] [Range(1f, 5f)] private float suggestionsChangeEvery = 1f;

		[SerializeField] private bool verbose;
#pragma warning restore 649
		#endregion

		public bool IsVerbose => verbose;

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

		/// <summary>
		/// Loads the scene with the specified <paramref name="sceneIndex"/> and activates it as soon as it's available.
		/// </summary>
		/// <param name="sceneIndex">Build index of the scene to load.</param>
		private void load(int sceneIndex) {
			gameObject.SetActive(true);
			if(suggestionsLabel) suggestionsCoroutine = Coroutiner.StartCoroutine(updateSuggestions(), "Loading suggestions", true);
			Coroutiner.StartCoroutine(doLoad(sceneIndex), "Loading scene");
		}

		/// <summary>
		/// Loads the scene with the specified <paramref name="sceneIndex"/> and executes the <paramref name="onReadyToActivate"/> callback when it's fully loaded and ready to activate.
		/// </summary>
		/// <param name="sceneIndex">Build index of the scene to load.</param>
		/// <param name="onReadyToActivate">Callback to execute when the scene is ready to be activated.</param>
		private void load(int sceneIndex, Action<AsyncOperation> onReadyToActivate) {
			gameObject.SetActive(true);
			if(suggestionsLabel) suggestionsCoroutine = Coroutiner.StartCoroutine(updateSuggestions(), "Loading suggestions", true);
			Coroutiner.StartCoroutine(doLoad(sceneIndex, onReadyToActivate), "Loading scene");
		}

		/// <summary>
		/// Loads the scene with the specified <paramref name="scenePath"/> and executes the <paramref name="onReadyToActivate"/> callback when it's fully loaded and ready to activate.
		/// </summary>
		/// <param name="scenePath">Build index of the scene to load.</param>
		/// <param name="onReadyToActivate">Callback to execute when the scene is ready to be activated.</param>
		private void load(string scenePath, Action<AsyncOperation> onReadyToActivate) {
			gameObject.SetActive(true);
			if(suggestionsLabel) suggestionsCoroutine = Coroutiner.StartCoroutine(updateSuggestions(), "Loading suggestions", true);
			Coroutiner.StartCoroutine(doLoad(scenePath, onReadyToActivate), "Loading scene");
		}

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

			progressSlider.value = 1f;
			onReadyToActivate(asyncLoad);
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

		private IEnumerator updateSuggestions() {
			if(suggestionsToShow == null) {
				initSuggestions();
			}
			if(suggestionsLabel && suggestionsToShow != null && suggestionsToShow.Count > 0) {
				while(true) {
					suggestionsLabel.text = suggestionsToShow[Random.Range(0, suggestionsToShow.Count - 1)];
					yield return new WaitForSeconds(suggestionsChangeEvery);
				}
			} else {
				this.DebugLog("There are no suggestions to show.");
				suggestionsLabel.text = string.Empty;
			}
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
	}
}