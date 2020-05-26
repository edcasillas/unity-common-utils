using System;
using CommonUtils.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CommonUtils.UI {
	/// <summary>
	/// A component that can be used to create a button that links to a random application (picked from its configuration.) using platform-dependent URLs.
	/// </summary>
	[AddComponentMenu("UI/Promo Badge")]
	[RequireComponent(typeof(Button), typeof(Image))]
	public class PromoBadge : MonoBehaviour {
		[Serializable]
		public class ApplicationData {
			public Sprite SourceImage;
			[FormerlySerializedAs("URL")]
			[Tooltip("URL to open on non-mobile platforms, and used as a fallback when platform-specific URLs have not been specified.")]
			public string FallbackUrl;
			public StringPerPlatformDictionary UrlPerPlatform;
		}

		public Image BadgeImage;
		public ApplicationData[] Apps;
		
		private ApplicationData appShown;

		private void Start() {
			if(Apps == null || Apps.Length == 0) {
				Debug.LogWarning($"{nameof(PromoBadge)} '{name}' doesn't have any apps set.", this);
				gameObject.SetActive(false);
				return;
			}

			appShown = Apps.PickRandom();
			BadgeImage.sprite = Apps.PickRandom().SourceImage;
		}

		public void Go() {
			var realUrl = appShown.UrlPerPlatform.ContainsKey(Application.platform) ? appShown.UrlPerPlatform[Application.platform] : appShown.FallbackUrl;

			if (string.IsNullOrWhiteSpace(realUrl)) {
				Debug.LogError("The current application doesn't have a URL defined for this platform and no fallback URL has been defined.", this);
				return;
			}
			
			Application.OpenURL(realUrl);
		}
	}
}