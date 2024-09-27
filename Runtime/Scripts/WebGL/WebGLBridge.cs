using CommonUtils.Logging;
using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;
using System.Runtime.CompilerServices;
using UnityEngine;
using ILogger = CommonUtils.Logging.ILogger;

namespace CommonUtils.WebGL {
	public partial class WebGLBridge : EnhancedMonoBehaviour, IWebGLBridge {
		#region Native functions
#if UNITY_WEBGL && !UNITY_EDITOR
	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void commonUtils_webGL_setVerbosity(int verbosity);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void commonUtils_webGL_goFullScreen();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern bool commonUtils_webGL_isMobileBrowser();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern string commonUtils_webGL_getUserAgent();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void commonUtils_webGL_setupPointerLockEvents(string gameObjectName);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void commonUtils_webGL_removePointerLockEvents();

	[System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void commonUtils_webGL_disableDefaultBehaviorForKey(string key);
#endif
		#endregion

		#region Static members
		#region Singleton definition
		private static bool instanceHasBeenResolved = false;
		private static IWebGLBridge instance;
		public static IWebGLBridge Instance {
			get {
				if (!instance.IsValid()) {
					if (instanceHasBeenResolved) {
						if (SingletonRegistry.TryResolve<ILogger>(out var logger)) {
							logger.Log(LogLevel.Error, $"Instance of {nameof(WebGLBridge)} has already been destroyed. This might happen when the application is being terminated.");
						}
						return null;
					}
					instance = FindObjectOfType<WebGLBridge>();
					if(instance.IsValid()) instance.Log2("WebGLBridge found in scene.", LogLevel.Warning);
					if (!instance.IsValid()) {
						instance = new GameObject(nameof(WebGLBridge)).AddComponent<WebGLBridge>();
						if(instance.IsValid()) instance.Log2("WebGLBridge not found in scene, a game object has been added to the scene.", LogLevel.Warning);
					}
					if(instance.IsValid()) instanceHasBeenResolved = true;
				}

				return instance;
			}
		}
		#endregion

		public static void GoFullScreen() {
#if UNITY_WEBGL && !UNITY_EDITOR
		instance.Log2("Requested full screen to the WebGL bridge.");
		commonUtils_webGL_goFullScreen();
#else
			instance.Log2("Requested full screen to the WebGL bridge. This will only work in WebGL builds.", LogLevel.Warning);
#endif
		}
		#endregion

		#region Inspector fields
#pragma warning disable CS0414
		[SerializeField] private bool mockMobileBrowser = false;
#pragma warning restore CS0414
		#endregion

		#region Properties
		[ShowInInspector]
		public bool IsMobileBrowser {
			get {
#if UNITY_WEBGL && !UNITY_EDITOR
			return commonUtils_webGL_isMobileBrowser();
#else
				return mockMobileBrowser;
#endif
			}
		}

		public string UserAgent {
			get {
#if UNITY_WEBGL && !UNITY_EDITOR
			return commonUtils_webGL_getUserAgent();
#else
				return string.Empty;
#endif
			}
		}

		private WebBrowserType? browserType = null;
		public WebBrowserType BrowserType {
			get {
				if (!browserType.HasValue) {
					var agent = UserAgent;
					if (string.IsNullOrEmpty(agent)) browserType = WebBrowserType.None;
					else if (agent.Contains("Mozilla/5.0")) browserType = WebBrowserType.Chrome;
					else if (agent.Contains("Chrome")) browserType = WebBrowserType.Chrome;
					else if (agent.Contains("Firefox")) browserType = WebBrowserType.Firefox;
					else if (agent.Contains("Safari")) browserType = WebBrowserType.Safari;
					else if (agent.Contains("Edge")) browserType = WebBrowserType.Edge;
					else if (agent.Contains("Opera")) browserType = WebBrowserType.Opera;
					else browserType = WebBrowserType.Unknown;
				}
				return browserType.Value;
			}
		}
		#endregion

		#region Unity Lifecycle
		private void Awake() {
			this.Log("Awake");
			#region Singleton check
			if (instance.IsValid() && instance != this as IWebGLBridge) {
				this.Log($"Multiple WebGL bridge instances are active. Instance is {instance.name}.", LogLevel.Warning);
				Destroy(gameObject);
				return;
			}

			instance = this;
			DontDestroyOnLoad(gameObject);
			#endregion
		}

		private void Start() {
#if UNITY_EDITOR
			if (mockMobileBrowser) {
				// Logged as error so it goes to the console despite IsVerbose.
				this.Log("Mocking mobile browser. To disable, uncheck this option in the WebGLBridge inspector.", LogLevel.Error);
			}
#endif
			this.Log(() => $"Playing game on {BrowserType}");
			setNativeVerbosity();
			if (!IsMobileBrowser) setupPointerLockEvents();
			DisableDefaultBehavior(KeyCode.Escape);
		}

		private void OnDestroy() {
			this.Log($"{nameof(WebGLBridge)} is being destroyed!");
			if(instance != this as IWebGLBridge) return;

			if(!IsMobileBrowser) removePointerLockEvents();
			instance = null;
		}
		#endregion

		// Not sure this works as expected
		public void DisableDefaultBehavior(KeyCode keyCode) {
			this.Log($"{nameof(DisableDefaultBehavior)}({keyCode})");
#if UNITY_WEBGL && !UNITY_EDITOR
            string keyString = keyCode.ToString(); // Convert KeyCode to string representation
            this.Log($"Disabling default behavior for {keyString}");
            commonUtils_webGL_disableDefaultBehaviorForKey(keyString);
#endif
		}

		private void setNativeVerbosity() {
			var nativeVerbosity = (int)Verbosity;
			if (nativeVerbosity < 0) nativeVerbosity = (int)(Verbosity.Debug | Verbosity.Warning | Verbosity.Error);
			this.Log($"Verbosity will be set to {nativeVerbosity}");
#if UNITY_WEBGL && !UNITY_EDITOR
			commonUtils_webGL_setVerbosity((int)nativeVerbosity);
#endif
		}
	}
}