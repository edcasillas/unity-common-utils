using CommonUtils.Extensions;
using CommonUtils.RestSdk;
using CommonUtils.WebResources;
using UnityEngine;
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
		[SerializeField] private AppDataCollectionConfigurator testConfig;
#pragma warning restore 649
		#endregion

		#region Properties and backing fields
		private Button _button;

		private Button button {
			get {
				if (!_button) _button = GetComponent<Button>();
				return _button;
			}
		}
		#endregion

		#region Private fields
		private string targetUrl;
		private IRestClient restClient;
		#endregion

		private void Awake() {
			loadingOverlay.SetActive(true);
			badgeContainer.SetActive(false);
			button.interactable = false;

			restClient = new RestClient(remoteConfigUrl);
		}

		private void Start() {
			restClient.Get<AppDataCollection>(string.Empty,
				result => {
					if (!result.IsSuccess) {
						gameObject.SetActive(false);
						return;
					}
					showApp(result.Data.Apps.PickRandom());
				});

			//if(!testConfig || testConfig.AppData?.Apps == null || testConfig.AppData.Apps.Length == 0) {
			//	Debug.LogWarning($"{nameof(PromoBadge)} '{name}' doesn't have any apps set.", this);
			//	gameObject.SetActive(false);
			//	return;
			//}
		}

		private void showApp(AppData appToShow) {
			targetUrl = appToShow.UrlPerPlatform.ContainsKey(Application.platform) ? appToShow.UrlPerPlatform[Application.platform] : appToShow.FallbackUrl;
			if (string.IsNullOrEmpty(targetUrl)) {
				gameObject.SetActive(false);
				return;
			}

			WebLoader.LoadWebTexture(appToShow.ImageUrl,
				response => {
					loadingOverlay.SetActive(false);
					if (!response.IsSuccess || !(response.Data is Texture2D tex)) return;
					badgeImage.sprite = Sprite.Create(tex,
						new Rect(0.0f, 0.0f, tex.width, tex.height),
						new Vector2(0.5f, 0.5f),
						100.0f);
					badgeContainer.SetActive(true);
					button.interactable = true;
				});
		}

		public void Go() => Application.OpenURL(targetUrl);
	}
}