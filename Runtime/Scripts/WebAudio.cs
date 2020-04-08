using CommonUtils.WebResources;
using UnityEngine;

namespace CommonUtils {
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("Audio/Web Audio")]
	public class WebAudio : AbstractWebResourceComponent {
#pragma warning disable 649
		[SerializeField] private bool playOnLoaded;
#pragma warning restore 649

		#region Properties and backing fields
		private AudioSource audioSource;

		public AudioSource AudioSource {
			get {
				if (!audioSource) audioSource = GetComponent<AudioSource>();
				return audioSource;
			}
		}
		#endregion

		protected override void Awake() {
			AudioSource.playOnAwake = false;
			base.Awake();
		}

		public override void Load() {
			base.Load();
			WebLoader.LoadWebAudioClip(Url,
				response => {
					if (response.IsSuccess) {
						AudioSource.clip = response.Data;
						Status = DownloadStatus.Loaded;
						OnResourceReady.Invoke();

						if(playOnLoaded) AudioSource.Play();
					} else {
						Status = DownloadStatus.Error;
					}
				});
		}
	}
}