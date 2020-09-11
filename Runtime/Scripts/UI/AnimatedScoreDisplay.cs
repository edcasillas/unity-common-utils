using System;
using CommonUtils.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CommonUtils.UI {
	/// <summary>
	/// Controls a <see cref="Text"/> component to show a numeric score that can be changed with an animation.
	/// </summary>
	[AddComponentMenu("UI/Animated Score Display")]
	[RequireComponent(typeof(Text))]
	public class AnimatedScoreDisplay : MonoBehaviour, IVerbosable {
		#region Inspector fields
		#pragma warning disable 649
		[SerializeField] private float           AnimDuration = 1f;
		[SerializeField] private iTween.EaseType EaseType;
		[SerializeField] private AudioClip       LoopSoundEffect;
		[SerializeField] private UnityEvent      OnAnimationFinished;
		[SerializeField] private bool verbose;
		#pragma warning restore 649
		#endregion

		#region Properties
		private Text _textComponent;
		private Text textComponent {
			get {
				if (_textComponent == null) _textComponent = GetComponent<Text>();
				return _textComponent;
			}
		}

		private AudioSource _audioSource;
		private AudioSource audioSource {
			get {
				if (_audioSource == null) {
					_audioSource = GetComponent<AudioSource>();
					if (_audioSource == null) {
						_audioSource = gameObject.AddComponent<AudioSource>();
					}
				}
				return _audioSource;
			}
		}

		public bool IsVerbose => verbose;
		#endregion

		private int currentScore;
		private Func<int, string> labelDelegate = s => s.ToString();

		private void Start() {
			if (LoopSoundEffect == null) return;
			audioSource.clip        = LoopSoundEffect;
			audioSource.loop        = true;
			audioSource.playOnAwake = false;
		}

		public void SetLabelDelegate(Func<int, string> newLabelDelegate) => labelDelegate = newLabelDelegate;

		public void SetScore(int newScore, bool animate = true) {
			this.DebugLog(() => $"{name}:AnimatedScoreDisplay.SetScore [current: {currentScore}] [new: {newScore}] [animate: {animate}]");
			var lastScore = currentScore;
			currentScore = newScore;

			if (!animate) {
				onAnimationCompleted();
				return;
			}

			playSfx(true);

			iTween.ValueTo(gameObject, iTween.Hash(
				"from", lastScore,
				"to", newScore,
				"time", AnimDuration,
				"onupdatetarget", gameObject,
				"onupdate", nameof(onAnimationUpdated),
				"oncompletetarget", gameObject,
				"oncomplete", nameof(onAnimationCompleted),
				"easeType", EaseType,
				"ignoretimescale", true
			));
		}

		private void onAnimationUpdated(int updatedValue) => textComponent.text = labelDelegate.Invoke(updatedValue);

		private void onAnimationCompleted() {
			textComponent.text = labelDelegate.Invoke(currentScore);
			playSfx(false);
			OnAnimationFinished.Invoke();
		}

		/// <summary>
		/// Plays or stops the sound effect
		/// </summary>
		/// <param name="play"></param>
		private void playSfx(bool play) {
			if (LoopSoundEffect != null) {
				if (play) audioSource.Play();
				else audioSource.Stop();
			}
		}
	}
}