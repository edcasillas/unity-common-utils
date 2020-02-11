using UnityEditor;

namespace Packages.CommonUtils.Editor {
	public static class MenuItems {
		[MenuItem("Tools/Common Utils/Free Packages/Missing References Finder", priority = 0)]
		private static void missingReferencesFinder() { Help.BrowseURL("https://github.com/edcasillas/unity-missing-references-finder"); }
		
		[MenuItem("Tools/Common Utils/Free Packages/Prefab Replacer", priority = 1)]
		private static void prefabReplacer() { Help.BrowseURL("https://github.com/edcasillas/unity-prefab-replacer"); }
		
		[MenuItem("Tools/Common Utils/Free Assets/Layer Selection Tool", priority = 2)]
		private static void layerSelectionTool() {
			Help.BrowseURL("https://assetstore.unity.com/packages/tools/utilities/layer-selection-tool-46068");
		}
		
		[MenuItem("Tools/Common Utils/Free Assets/Asset Usage Detector", priority = 3)]
		private static void assetUsageDetector() {
			Help.BrowseURL("https://assetstore.unity.com/packages/tools/utilities/asset-usage-detector-112837");
		}
	}
}