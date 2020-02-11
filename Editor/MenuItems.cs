using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.CommonUtils.Editor {
	public static class MenuItems {
		[MenuItem("Tools/Common Utils/Free Assets/Layer Selection Tool")]
		private static void layerSelectionTool() {
			string path = EditorUtility.OpenFilePanel("Select manifest.json", "", "png");
			Debug.Log(path);
			Help.BrowseURL("https://assetstore.unity.com/packages/tools/utilities/layer-selection-tool-46068");
		}
		
		[MenuItem("Tools/Common Utils/Free Packages/Missing References Finder")]
		private static void missingReferencesFinder() { UnityEditor.PackageManager.Client.Add("https://github.com/edcasillas/unity-missing-references-finder.git"); }
	}
}