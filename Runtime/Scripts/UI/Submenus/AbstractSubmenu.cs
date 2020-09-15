using System.Collections;
using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.UI.Submenus {
    [RequireComponent(typeof(RectTransform), typeof(AudioSource))]
	public abstract class AbstractSubmenu : MonoBehaviour , IVerbosable{
		#region Inspector fields
#pragma warning disable 649
		[Tooltip("Ease animation type when showing the submenu.")]
		public iTween.EaseType EaseIn;
		[Tooltip("Ease animation type when hiding the submenu.")]
		public iTween.EaseType EaseOut;
		[Range(0, 1)]
		public float AnimDuration = 0.5f;
		public AudioClip Feedback;
		public bool PlayFeedbackOnShow;
		public bool PlayFeedbackOnHide;
		[Range(0, 20)]
		public int AutoHide = 0;
		[SerializeField] private bool verbose;
#pragma warning restore 649
		#endregion

		#region Properties
		public bool IsInitialized { get; private set; } = false;
		public bool IsShown { get; private set; } = false;

		public Vector2 HiddenValue { get; protected set; }
		public Vector2 ShownValue { get; protected set; }

		public bool IsVerbose => verbose;
		#endregion

		protected RectTransform RectTransform;
		protected AudioSource AudioSource;
		private Coroutine hideCoroutine;

		#region Public Methods

		public virtual void Show() {
			this.DebugLog(() => $"Will show {name}");
			Init();
			if(hideCoroutine != null) {
				this.DebugLog("Stopping hide coroutine.");
				StopCoroutine(hideCoroutine);
				if(IsShown && AutoHide > 0) {
					hideCoroutine = StartCoroutine(waitAndHide());
				}
			}
			if(!IsShown) {
				IsShown = true;
				gameObject.SetActive(true);
				animate(HiddenValue, ShownValue, nameof(OnShown), EaseIn, PlayFeedbackOnShow);
			}
		}

		public virtual void Hide() {
			if (!IsInitialized) {
				Debug.LogError($"Cannot hide this submenu before is initialized. Please call Init(); first.");
			}
			this.DebugLog($"Will hide {name}");
			IsShown = false;
			animate(ShownValue, HiddenValue, nameof(OnHidden), EaseOut, PlayFeedbackOnHide);
		}

		#endregion

		#region Abstract Methods

		protected abstract void OnInit();

		public abstract void OnAnimationUpdated(Vector2 updatedValue);

		#endregion

		#region Virtual Methods
		protected virtual void OnShown() {
			if(AutoHide > 0) {
				this.DebugLog("Starting hide coroutine.");
				hideCoroutine = StartCoroutine(waitAndHide());
			}
		}

		protected virtual void OnHidden() {
			gameObject.SetActive(false);
		}

		#endregion

		#region Private Methods

		public void Init() {
			if(!IsInitialized) {
				RectTransform = GetComponent<RectTransform>();
				AudioSource = GetComponent<AudioSource>();
				OnInit();
				IsInitialized = true;
			}
		}

		private void animate(Vector2 start, Vector2 end, string onComplete, iTween.EaseType easeType, bool playSound) {
			iTween.ValueTo(gameObject, iTween.Hash(
				"from", start,
				"to", end,
				"time", AnimDuration,
				"onupdatetarget", gameObject,
				"onupdate", nameof(OnAnimationUpdated),
				"oncompletetarget", gameObject,
				"oncomplete", onComplete,
				"easeType", easeType,
				"ignoretimescale", true
			));
			if(playSound) {
				this.PlayFeedback();
			}
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

		private IEnumerator waitAndHide() {
			yield return new WaitForSeconds(AutoHide);
			this.DebugLog("Hiding");
			Hide();
			hideCoroutine = null;
		}

		#endregion
	}
}