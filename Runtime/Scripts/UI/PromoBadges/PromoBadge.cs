using System;
using CommonUtils.Extensions;
using CommonUtils.WebResources;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CommonUtils.UI.PromoBadges {
	/// <summary>
	/// A component that can be used to create a button that links to a random application (picked from its configuration.) using platform-dependent URLs.
	/// </summary>
	[AddComponentMenu("UI/Promo Badge")]
	[RequireComponent(typeof(Button))]
	public class PromoBadge : MonoBehaviour {
		[Serializable]
		public class ApplicationData {
			public Sprite SourceImage;
			[FormerlySerializedAs("URL")]
			[Tooltip("URL to open on non-mobile platforms, and used as a fallback when platform-specific URLs have not been specified.")]
			public string FallbackUrl;
			public StringPerPlatformDictionary UrlPerPlatform;
		}

		#region Inspector fields
		#pragma warning disable 649
		[SerializeField] private GameObject badgeContainer;
		[SerializeField] private GameObject loadingOverlay;
		[FormerlySerializedAs("BadgeImage")]
		[SerializeField] private Image badgeImage;
		[SerializeField] private string remoteConfigUrl;
		[SerializeField] private AppDataCollection testConfig;
		public ApplicationData[] Apps;
		#pragma warning restore 649
		#endregion
		
		private ApplicationData appShown;

		private Button _button;

		private Button button {
			get {
				if (!_button) _button = GetComponent<Button>();
				return _button;
			}
		}

		private void Awake() {
			loadingOverlay.SetActive(true);
			badgeContainer.SetActive(false);
			button.interactable = false;
		}

		private void Start() {
			if(Apps == null || Apps.Length == 0) {
				Debug.LogWarning($"{nameof(PromoBadge)} '{name}' doesn't have any apps set.", this);
				gameObject.SetActive(false);
				return;
			}

			var newAppShown = testConfig.Apps.PickRandom();
			WebLoader.LoadWebTexture(newAppShown.ImageUrl,
				response => {
					loadingOverlay.SetActive(false);
					if (!response.IsSuccess || !(response.Data is Texture2D tex)) return;
					appShown = new ApplicationData {FallbackUrl = newAppShown.FallbackUrl, UrlPerPlatform = newAppShown.UrlPerPlatform};
					badgeImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
					badgeContainer.SetActive(true);
					button.interactable = true;
				});
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