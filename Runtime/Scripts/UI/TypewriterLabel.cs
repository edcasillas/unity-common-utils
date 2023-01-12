using CommonUtils.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils.UI {
	[RequireComponent(typeof(TMP_Text))]
	public class TypewriterLabel : EnhancedMonoBehaviour {
		[SerializeField] [TextArea] private string text;
		[SerializeField] private float totalTimeToWrite = 1;
		[SerializeField] private AudioClip[] feedbackStrokes;
		[SerializeField] private bool playOnStart;
		[SerializeField] private bool clearOnPlay = true;
		[SerializeField] private UnityEvent onFinished;

		private TMP_Text label;
		private AudioSource audioSource;
		private float timeBetweenTypes;
		private Coroutine coroutine;

		private void Awake() {
			label = GetComponent<TMP_Text>();
			audioSource = GetComponent<AudioSource>();
		}

		private void Start() {
			if(playOnStart) Play();
		}

		public void Play(string text) {
			this.text = text;
			Play();
		}

		[ShowInInspector]
		public void Play() {
			if (coroutine != null) {
				StopCoroutine(coroutine);
			}

			if (clearOnPlay) label.text = string.Empty;
			timeBetweenTypes = totalTimeToWrite / text.Length;
			coroutine = StartCoroutine(playingCoroutine());
		}

		[ShowInInspector]
		public void FastForward() {
			if (coroutine != null) {
				StopCoroutine(coroutine);
			}

			label.text = text;
		}

		private IEnumerator playingCoroutine() {
			var i = 0;
			while (i < text.Length) {
				label.text += text[i];
				audioSource.PlayOneShot(feedbackStrokes.PickRandom());
				yield return new WaitForSeconds(timeBetweenTypes);
				i++;
			}

			coroutine = null;
			onFinished?.Invoke();
		}
	}
}