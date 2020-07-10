using CommonUtils.Serializables.SerializableDictionaries;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI {
	[AddComponentMenu("UI/Hyperlink Button")]
	[RequireComponent(typeof(Button))]
	public class HyperlinkButton : MonoBehaviour {
		#pragma warning disable 649
		[SerializeField] private StringPerPlatformDictionary platformSpecificUrls;
		[SerializeField] private string fallbackUrl;
#pragma warning restore 649 

		public void Go() {
			string realUrl;
			realUrl = platformSpecificUrls.ContainsKey(Application.platform) ? platformSpecificUrls[Application.platform] : fallbackUrl;
			Application.OpenURL(realUrl);
		}

		private void Reset() => GetComponent<Button>().onClick.AddListener(Go);
	}
}