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
		#region Inspector fields
#pragma warning disable 649
		[SerializeField] private GameObject badgeContainer;
		[SerializeField] private GameObject loadingOverlay;
		[SerializeField] private Image badgeImage;
		[SerializeField] private string remoteConfigUrl;
		[SerializeField] private AppDataCollection testConfig;
#pragma warning restore 649
		#endregion
		
		private string targetUrl;

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
			if(testConfig?.Apps == null || testConfig.Apps.Length == 0) {
				Debug.LogWarning($"{nameof(PromoBadge)} '{name}' doesn't have any apps set.", this);
				gameObject.SetActive(false);
				return;
			}

			var newAppShown = testConfig.Apps.PickRandom();
			targetUrl = newAppShown.UrlPerPlatform.ContainsKey(Application.platform) ? newAppShown.UrlPerPlatform[Application.platform] : newAppShown.FallbackUrl;
			if (string.IsNullOrEmpty(targetUrl)) {
				gameObject.SetActive(false);
				return;
			}

			WebLoader.LoadWebTexture(newAppShown.ImageUrl,
				response => {
					loadingOverlay.SetActive(false);
					if (!response.IsSuccess || !(response.Data is Texture2D tex)) return;
					badgeImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
					badgeContainer.SetActive(true);
					button.interactable = true;
				});
		}

		public void Go() => Application.OpenURL(targetUrl);
	}
}