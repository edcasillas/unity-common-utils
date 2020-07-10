using System;
using CommonUtils.Serializables.SerializableDictionaries;

namespace CommonUtils.UI.PromoBadges {
	[Serializable]
	internal class AppData {
		public string Name;
		public string ImageUrl;
		public string FallbackUrl;
		public StringPerPlatformDictionary UrlPerPlatform;
	}
}