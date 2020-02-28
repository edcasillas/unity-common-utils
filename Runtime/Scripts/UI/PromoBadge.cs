using UnityEngine;
using System;
using CommonUtils.Extensions;
using UnityEngine.UI;

namespace ExaGames.Common {
	/// <summary>
	/// A component that can be used to create a button that links to a random application (picked from its configuration.) using platform-dependent URLs.
	/// </summary>
	[AddComponentMenu("UI/Promo Badge")]
	[RequireComponent(typeof(Button), typeof(Image))]
	public class PromoBadge : MonoBehaviour {
		[Serializable]
		public class ApplicationData {
			public Sprite SourceImage;
			[Tooltip("URL to open on non-mobile platforms, and used as a fallback when platform-specific URLs have not been specified.")]
			public string URL;
			public string AndroidSpecificURL;
			public string IOSSpecificURL;
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
			string realUrl;
			if(Application.platform == RuntimePlatform.Android && !string.IsNullOrWhiteSpace(appShown.AndroidSpecificURL)) {
				realUrl = appShown.AndroidSpecificURL;
			} else if(Application.platform == RuntimePlatform.IPhonePlayer && !string.IsNullOrWhiteSpace(appShown.IOSSpecificURL)) {
				realUrl = appShown.IOSSpecificURL;
			} else {
				realUrl = appShown.URL;
			}

			if (string.IsNullOrWhiteSpace(realUrl)) {
				Debug.LogError("The current application doesn't have a URL defined for this platform and no fallback URL has been defined.", this);
				return;
			}
			
			Application.OpenURL(realUrl);
		}
	}
}