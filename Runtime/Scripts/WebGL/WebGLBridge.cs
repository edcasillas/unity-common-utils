using CommonUtils;
using CommonUtils.Logging;
using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;
using System.Collections;
using UnityEngine;

public interface IWebGLBridge : IUnityComponent {
	bool IsMobileBrowser { get; }
}

public class WebGLBridge : EnhancedMonoBehaviour, IWebGLBridge {
	#region Static members
	#if UNITY_WEBGL && !UNITY_EDITOR
	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void goFullScreen();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern bool isMobileBrowser();
	#endif

	#region Singleton definition
	private static IWebGLBridge instance;
	public static IWebGLBridge Instance {
		get {
			if (!instance.IsValid()) {
				instance = FindObjectOfType<WebGLBridge>();
				if (!instance.IsValid()) {
					instance = new GameObject().AddComponent<WebGLBridge>();
				}
			}

			return instance;
		}
	}
	#endregion

	public static void GoFullScreen() {
		#if UNITY_WEBGL && !UNITY_EDITOR
		Debug.Log("Requested full screen to the WebGL bridge.");
		goFullScreen();
		#else
		Debug.LogWarning("Requested full screen to the WebGL bridge. This will only work in WebGL builds.");
		#endif
	}
	#endregion

	#region Inspector fields
	[SerializeField] private BoolEvent onPointerLockChanged;
	[SerializeField] private float timeToWaitForPointerLockedEvent = 0.05f;

	#pragma warning disable CS0414
	[SerializeField] private bool mockMobileBrowser = false;
	#pragma warning restore CS0414
	#endregion

	#region Properties
	[ShowInInspector]
	public bool IsMobileBrowser {
		get {
#if UNITY_WEBGL && !UNITY_EDITOR
			return isMobileBrowser();
#else
			return mockMobileBrowser;
#endif
		}
	}

	private bool pointerIsLocked;
	public bool PointerIsLocked {
		get => pointerIsLocked;
		private set {
			if (pointerIsLocked == value) return;
			pointerIsLocked = value;
			this.Log($"Pointer is now {(pointerIsLocked ? "locked" : "unlocked")}");
			onPointerLockChanged?.Invoke(pointerIsLocked);
		}
	}
	#endregion

	private bool wantedNewPointerIsLockedValue;
	private bool receivedFollowUp = false;

	#region Unity Lifecycle
	private void Awake() {
		#region Singleton check
		if (instance.IsValid() && !ReferenceEquals(instance, this)) {
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
	}

	private void OnDestroy() => instance = null;
	#endregion

	[ShowInInspector]
	public void OnPointerLockChanged(int locked) {
		if (locked > 0) {
			PointerIsLocked = true;
			receivedFollowUp = true;
			return;
		}

		StartCoroutine(handlePointerUnlocked());
	}

	/// <summary>
	/// Waits for a bit to see if there's a second call to OnPointerLockChanged, and does not set it as unlocked if
	/// another call arrived in a short amount of time.
	/// </summary>
	/// <returns></returns>
	private IEnumerator handlePointerUnlocked() {
		receivedFollowUp = false;
		yield return new WaitForSeconds(timeToWaitForPointerLockedEvent);
		if(receivedFollowUp) yield break;
		PointerIsLocked = false;
	}
}