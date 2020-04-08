using System;
using CommonUtils.WebResources;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI {
	[RequireComponent(typeof(RawImage))]
	[AddComponentMenu("UI/Web Image")]
	public class WebImage : MonoBehaviour {
		#region Inspector fields
#pragma warning disable 649
		[SerializeField] private string url;
		[SerializeField] private bool loadOnAwake;
		[SerializeField] private Texture errorTexture;
#pragma warning restore 649
		#endregion

		#region Properties and backing fields
		public string Url { get => url; set => url = value; }

		private RawImage _rawImage;

		private RawImage rawImage {
			get {
				if (!_rawImage) _rawImage = GetComponent<RawImage>();
				return _rawImage;
			}
		}

		public DownloadStatus Status { get; private set; } = DownloadStatus.NotInited;
		#endregion

		private void Awake() {
			if(loadOnAwake) Load();
		}

		public void Load() {
			/*if (string.IsNullOrWhiteSpace(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
				Debug.LogError($"A valid URL has not been set to load a web image.", this);
				return;
			}*/
			Status = DownloadStatus.Loading;
			WebLoader.LoadWebTexture(url,
				response => {
					if (response.IsSuccess) {
						rawImage.texture = response.Data;
						Status = DownloadStatus.Loaded;
					} else if (errorTexture) {
						rawImage.texture = errorTexture;
						Status = DownloadStatus.Error;
					}
				});
		}
	}
}