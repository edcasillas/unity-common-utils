using CommonUtils.Extensions;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CommonUtils.UI {
    /// <summary>
    /// Controls a <see cref="Text"/> component to show a numeric score that can be changed with an animation.
    /// </summary>
    public abstract class AbstractAnimatedScoreDisplay : MonoBehaviour, IVerbosable {
        #region Inspector fields
#pragma warning disable 649
        [SerializeField] private float AnimDuration = 1f;
        [SerializeField] private iTween.EaseType EaseType;
        [SerializeField] private AudioClip LoopSoundEffect;
        [SerializeField] private UnityEvent OnAnimationFinished;
        [SerializeField] private bool verbose;
#pragma warning restore 649
        #endregion

        #region Properties
        private AudioSource _audioSource;
        private AudioSource audioSource {
            get {
                if (!_audioSource) {
                    _audioSource = GetComponent<AudioSource>();
                    if (!_audioSource) {
                        _audioSource = gameObject.AddComponent<AudioSource>();
                    }
                }
                return _audioSource;
            }
        }

        public bool IsVerbose => verbose;
        #endregion

        private int currentScore;
        protected Func<int, string> LabelDelegate = s => s.ToString();

        private void Start() {
            if (LoopSoundEffect == null) return;
            audioSource.clip = LoopSoundEffect;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }

        public void SetLabelDelegate(Func<int, string> newLabelDelegate) => LabelDelegate = newLabelDelegate;

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
                "onupdate", nameof(OnAnimationUpdated),
                "oncompletetarget", gameObject,
                "oncomplete", nameof(onAnimationCompleted),
                "easeType", EaseType,
                "ignoretimescale", true
            ));
        }

        protected abstract void UpdateLabel(int score);

        protected void OnAnimationUpdated(int updatedValue) => UpdateLabel(updatedValue);

        private void onAnimationCompleted() {
            UpdateLabel(currentScore);
            playSfx(false);
            OnAnimationFinished.Invoke();
        }

        /// <summary>
        /// Plays or stops the sound effect
        /// </summary>
        /// <param name="play"></param>
        private void playSfx(bool play) {
            if (LoopSoundEffect) {
                if (play) audioSource.Play();
                else audioSource.Stop();
            }
        }
    }
}
