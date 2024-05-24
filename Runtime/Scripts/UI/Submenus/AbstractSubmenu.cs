using System.Collections;
using CommonUtils.Extensions;
using CommonUtils.Verbosables;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils.UI.Submenus {
	/// <summary>
	/// Base class for submenus animated through iTween.
	/// </summary>
    [RequireComponent(typeof(RectTransform), typeof(AudioSource))]
	public abstract class AbstractSubmenu : EnhancedMonoBehaviour, ISubmenu {
		[Serializable]
		public class SubMenuEvents {
			public UnityEvent OnShown;
			public UnityEvent OnHidden;
		}

		#region Inspector fields
#pragma warning disable 649
		/// <summary>
		/// Ease animation type when showing the submenu.
		/// </summary>
		[Tooltip("Ease animation type when showing the submenu.")]
		public iTween.EaseType EaseIn;

		/// <summary>
		/// Ease animation type when hiding the submenu.
		/// </summary>
		[Tooltip("Ease animation type when hiding the submenu.")]
		public iTween.EaseType EaseOut;

		/// <summary>
		/// Duration of in/out animations in seconds.
		/// </summary>
		public float AnimDuration = 0.5f;

		public AudioClip Feedback;
		public bool PlayFeedbackOnShow;
		public bool PlayFeedbackOnHide;
		[Range(0, 20)]
		public int AutoHide = 0;

		[SerializeField] private SubMenuEvents events;
#pragma warning restore 649
		#endregion

		#region Properties and backing fields
		[ShowInInspector] public bool IsInitialized { get; private set; } = false;

		/// <summary>
		/// Gets a value indicating whether this submenu is being shown.
		/// </summary>
		/// <remarks>Formerly called 'IsOpen'.</remarks>
		[ShowInInspector] public bool IsShown { get; protected set; } = false;

		[ShowInInspector] public Vector2 HiddenValue { get; protected set; }
		[ShowInInspector] public Vector2 ShownValue { get; protected set; }

		private RectTransform rectTransform;
		/// <summary>
		/// <see cref="RectTransform"/> of the submenu, to manipulate its size, position, etc.
		/// </summary>
		protected RectTransform RectTransform {
			get {
				if (!rectTransform) rectTransform = GetComponent<RectTransform>();
				return rectTransform;
			}
		}

		private AudioSource audioSource;
		protected AudioSource AudioSource {
			get {
				if (!audioSource) audioSource = GetComponent<AudioSource>();
				return audioSource;
			}
		}

		[ShowInInspector]
		protected Vector2 CurrentValue { get; private set; }
		#endregion

		#region Fields
		protected Coroutine HideCoroutine;

		private float distanceBetweenHiddenAndShown;
		#endregion

		#region Unity Lifecycle
		private void Start() => Init();
		#endregion

		#region Public Methods
		[ShowInInspector]
		public void Init() {
			if (IsInitialized) return;
			OnInit();

			distanceBetweenHiddenAndShown = Vector2.Distance(HiddenValue, ShownValue);

			if (gameObject.activeSelf) {
				IsShown = true;
				internalOnAnimationUpdated(ShownValue);
			} else {
				IsShown = false;
				internalOnAnimationUpdated(HiddenValue);
			}

			IsInitialized = true;
		}

		[ShowInInspector]
		public virtual void Show() {
			this.DebugLog(() => $"Will show {name}");
			if (EaseIn == iTween.EaseType.punch) {
				this.LogError("Ease type 'Punch' is not supported.");
				return;
			}
			Init();
			if(HideCoroutine != null) {
				this.DebugLog("Stopping hide coroutine.");
				StopCoroutine(HideCoroutine);
				if(IsShown && AutoHide > 0) {
					HideCoroutine = StartCoroutine(WaitAndHide());
				}
			}
			if(!IsShown) {
				IsShown = true;
				gameObject.SetActive(true);
				animate(CurrentValue, ShownValue, nameof(OnShown), EaseIn, PlayFeedbackOnShow);
			}
		}

		[ShowInInspector]
		public virtual void Hide() {
			if (!IsInitialized) {
				this.LogError($"Cannot hide submenu '{name}' before is initialized. Please call Init(); first.");
				return;
			}

			if (EaseOut == iTween.EaseType.punch) {
				this.LogError("Ease type 'Punch' is not supported.");
				return;
			}

			if(!IsShown) return;
			this.DebugLog($"Will hide {name}");
			IsShown = false;
			animate(CurrentValue, HiddenValue, nameof(OnHidden), EaseOut, PlayFeedbackOnHide);
		}

		/// <summary>
		/// Hides this submenu without performing any animations or sounds.
		/// </summary>
		/// <remarks>
		/// Use this method instead of GameObject.SetActive(false) to keep consistency with the data of the submenu
		/// to be able to show and hide again with animations.
		/// </remarks>
		[ShowInInspector]
		public void HideImmediately() {
			if(!IsShown) return;
			this.DebugLog("Hiding without animation.");
			iTween.Stop(gameObject);
			IsShown = false;
			internalOnAnimationUpdated(HiddenValue);
			OnHidden();

		}

		public void SubscribeOnShown(UnityAction action) => events.OnShown.AddListener(action);
		public void SubscribeOnHidden(UnityAction action) => events.OnHidden.AddListener(action);
		public void UnsubscribeOnShown(UnityAction action) => events.OnShown.RemoveListener(action);
		public void UnsubscribeOnHidden(UnityAction action) => events.OnHidden.RemoveListener(action);
		#endregion

		#region Abstract Methods
		protected abstract void OnInit();
		public abstract void OnAnimationUpdated(Vector2 updatedValue);
		#endregion

		#region Virtual Methods
		protected virtual void OnShown() {
			if(AutoHide > 0) {
				this.DebugLog($"Starting hide coroutine for submenu {name}.");
				HideCoroutine = StartCoroutine(WaitAndHide());
			}
			events.OnShown?.Invoke();
		}

		protected virtual void OnHidden() {
			gameObject.SetActive(false);
			events.OnHidden?.Invoke();
		}
		#endregion

		#region Private Methods
		private void animate(Vector2 start, Vector2 end, string onComplete, iTween.EaseType easeType, bool playSound) {
			iTween.Stop(gameObject);
			iTween.ValueTo(gameObject, iTween.Hash(
				"from", start,
				"to", end,
				"time", getProportionalAnimDuration(start, end),
				"onupdatetarget", gameObject,
				"onupdate", nameof(internalOnAnimationUpdated),
				"oncompletetarget", gameObject,
				"oncomplete", onComplete,
				"easeType", easeType,
				"ignoretimescale", true
			));
			if(playSound) {
				PlayFeedback();
			}
		}

		private float getProportionalAnimDuration(Vector2 start, Vector2 end) {
			var distance = Vector2.Distance(start, end);
			return (distance * AnimDuration) / distanceBetweenHiddenAndShown;
		}

		private void internalOnAnimationUpdated(Vector2 updatedValue) {
			CurrentValue = updatedValue;
			OnAnimationUpdated(updatedValue);
		}

		protected void PlayFeedback() {
			if(!AudioSource) {
				Debug.LogError($"{name} doesn't have an AudioSource.", this);
			}
			if(!Feedback) {
				Debug.LogError($"{name} doesn't have a feedback AudioClip.", this);
			}
			AudioSource.PlayOneShot(Feedback);
		}

		protected virtual IEnumerator WaitAndHide() { // When overriden, please make sure to set HideCoroutine to null!
			yield return new WaitForSeconds(AutoHide);
			this.DebugLog("Hiding");
			Hide();
			HideCoroutine = null;
		}
		#endregion
	}
}