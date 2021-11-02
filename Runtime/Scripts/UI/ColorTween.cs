using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI {
	/*
	 * TODO This component has a similar effect as using BlinkerUIElements. Maybe need to reconsider it.
	 */
	[RequireComponent(typeof(Image))]
	public class ColorTween : MonoBehaviour {
		#region Public inspector properties
		public Color TargetColor;
		public iTween.EaseType EaseType = iTween.EaseType.linear;

		[Range(0, 10)]
		public float AnimDuration = 4f;

		public bool PlayOnEnable = true;
		public bool Loop = false;
		public bool IncludeChildren = false;
		#endregion

		private Color startColor;
		private ICollection<Image> imageComponents;
		
		/// <summary>
		/// When <c>true</c>, color is being animated from start to target. When <c>false</c>, color is being animated from target to start.
		/// </summary>
		private bool direction;

		protected virtual void Awake() {
			var imageComponent = GetComponent<Image>();
			startColor = imageComponent.color;

			if (IncludeChildren) { imageComponents = GetComponentsInChildren<Image>(); }
			else { imageComponents = new List<Image>() { imageComponent }; }
		}

		private void OnEnable() { if (PlayOnEnable) PlayStartToTarget(); }

		private void OnDisable() { iTween.Stop(gameObject); }

		public void PlayStartToTarget() {
			direction = true;
			animate(startColor, TargetColor);
		}

		public void PlayTargetToStart() {
			direction = false;
			animate(TargetColor, startColor);
		}

		private void animate(Color from, Color to) {
			iTween.ValueTo(gameObject, iTween.Hash(
				"from", from,
				"to", to,
				"time", AnimDuration,
				"onupdatetarget", gameObject,
				"onupdate", "onAnimationUpdated",
				"oncompletetarget", gameObject,
				"oncomplete", "onAnimationFinished",
				"easeType", EaseType,
				"ignoretimescale", true
			));
		}

		private void onAnimationUpdated(Color updatedColor) {
			foreach (var image in imageComponents) {
				image.color = updatedColor;
			}
		}

		private void onAnimationFinished() {
			if (Loop) {
				if (direction) PlayTargetToStart();
				else PlayStartToTarget();
			}
		}
	}
}