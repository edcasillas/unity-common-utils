using CommonUtils.WebResources;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI {
	[RequireComponent(typeof(RawImage))]
	[AddComponentMenu("UI/Web Image")]
	public class WebImage : AbstractWebResourceComponent {
		#region Inspector fields
#pragma warning disable 649
		[SerializeField] private Texture errorTexture;
#pragma warning restore 649
		#endregion

		#region Properties and backing fields
		private RawImage _rawImage;

		private RawImage rawImage {
			get {
				if (!_rawImage) _rawImage = GetComponent<RawImage>();
				return _rawImage;
			}
		}
		
		private Texture cachedTexture2D { get; set; }
		#endregion

		public override void Load() {
			base.Load();
			WebLoader.LoadWebTexture(Url,
				response => {
					if (response.IsSuccess) {
						cachedTexture2D = response.Data;
						rawImage.texture = cachedTexture2D;
						Status = DownloadStatus.Loaded;
						OnResourceReady.Invoke();
					} else {
						if (errorTexture) rawImage.texture = errorTexture;
						Status = DownloadStatus.Error;
					}
				});
		}

		private void OnDestroy() {
			if(cachedTexture2D) Destroy(cachedTexture2D);
		}
	}
}