using UnityEngine;
using UnityEngine.Serialization;

namespace CommonUtils.UI.Tween {
	public abstract class AbstractTweener<T> : EnhancedMonoBehaviour {
		#region Inspector fields
		[SerializeField] protected T targetValue;

		[FormerlySerializedAs("EaseType")]
		[SerializeField] protected iTween.EaseType easeType = iTween.EaseType.linear;

		[FormerlySerializedAs("AnimDuration")]
		[SerializeField] [Range(0, 10)] protected float animDuration = 4f;

		[FormerlySerializedAs("PlayOnEnable")]
		[SerializeField] protected bool playOnEnable = true;

		[FormerlySerializedAs("Loop")]
		[SerializeField] protected bool loop = false;
		#endregion

		#region Properties
		/// <summary>
		/// When <c>true</c>, color is being animated from start to target. When <c>false</c>, color is being animated from target to start.
		/// </summary>
		[ShowInInspector] protected bool Direction { get; private set; }

		[ShowInInspector] protected T StartValue { get; set; }
		#endregion

		#region Unity Lifecycle
		protected virtual void Awake() => StartValue = InitializeStartValue();

		protected void OnEnable() { if (playOnEnable) PlayStartToTarget(); }

		protected void OnDisable() => iTween.Stop(gameObject);
		#endregion

		#region Public (debuggable) methods
		[ShowInInspector]
		public void PlayStartToTarget() {
			Direction = true;
			animate(StartValue, targetValue);
		}

		[ShowInInspector]
		public void PlayTargetToStart() {
			Direction = false;
			animate(targetValue, StartValue);
		}
		#endregion

		protected abstract T InitializeStartValue();
		protected abstract void OnAnimationUpdated(T updatedValue);

		protected virtual void OnAnimationFinished() {
			if (!loop) return;
			if (Direction) PlayTargetToStart();
			else PlayStartToTarget();
		}

		private void animate(T from, T to) {
			iTween.ValueTo(gameObject, iTween.Hash(
				"from", from,
				"to", to,
				"time", animDuration,
				"onupdatetarget", gameObject,
				"onupdate", nameof(OnAnimationUpdated),
				"oncompletetarget", gameObject,
				"oncomplete", nameof(OnAnimationFinished),
				"easeType", easeType,
				"ignoretimescale", true
			));
		}
	}
}