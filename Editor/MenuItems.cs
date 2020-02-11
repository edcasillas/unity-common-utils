using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.CommonUtils.Editor {
	public static class MenuItems {
		[MenuItem("Tools/Common Utils/Free Assets/Layer Selection Tool")]
		private static void layerSelectionTool() {
			Help.BrowseURL("https://assetstore.unity.com/packages/tools/utilities/layer-selection-tool-46068");
		}
		
		[MenuItem("Tools/Common Utils/Free Packages/Missing References Finder")]
		private static void missingReferencesFinder() { Help.BrowseURL("https://github.com/edcasillas/unity-missing-references-finder"); }
	}
}