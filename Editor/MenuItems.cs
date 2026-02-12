using UnityEditor;

namespace CommonUtils.Editor {
	public static class MenuItems {
		public const string MenuPathPrefix = "Tools/Common Utils/";

		[MenuItem(MenuPathPrefix + "Free Packages/Missing References Finder", priority = 998)]
		private static void missingReferencesFinder() => Help.BrowseURL("https://github.com/edcasillas/unity-missing-references-finder");

		[MenuItem(MenuPathPrefix + "Free Packages/Prefab Replacer", priority = 998)]
		private static void prefabReplacer() => Help.BrowseURL("https://github.com/edcasillas/unity-prefab-replacer");

		[MenuItem(MenuPathPrefix + "Free Assets/Layer Selection Tool", priority = 998)]
		private static void layerSelectionTool() => Help.BrowseURL("https://assetstore.unity.com/packages/tools/utilities/layer-selection-tool-46068");

		[MenuItem(MenuPathPrefix + "Free Assets/Asset Usage Detector", priority = 998)]
		private static void assetUsageDetector() => Help.BrowseURL("https://assetstore.unity.com/packages/tools/utilities/asset-usage-detector-112837");

		[MenuItem(MenuPathPrefix + "Free Assets/PlayerPrefs Editor", priority = 998)]
		private static void playerPrefsEditor() => Help.BrowseURL("https://assetstore.unity.com/packages/tools/utilities/playerprefs-editor-167903");

		[MenuItem(MenuPathPrefix + "Go to Wiki", priority = 999)]
		private static void goToWiki() => Help.BrowseURL("https://github.com/edcasillas/unity-common-utils/wiki");
	}
}