using UnityEditor;
using UnityEngine;

namespace Packages.CommonUtils.Editor {
	public static class MenuItems {
		[MenuItem("Tools/Common Util Assets (free)/Layer Selection Tool")]
		private static void layerSelectionTool() {
			Help.BrowseURL("https://assetstore.unity.com/packages/tools/utilities/layer-selection-tool-46068");
		}
	}
}