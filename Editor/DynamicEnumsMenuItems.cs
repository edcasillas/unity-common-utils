using CommonUtils.DynamicEnums;
using UnityEditor;

namespace CommonUtils.Editor {
	public static class DynamicEnumsMenuItems {
		[MenuItem("Tools/Dynamic Enums/Configure", priority = 0)]
		private static void configure() {
			if (!EditorUtils.HighlightAssetOfType<DynamicEnumDefinitions>("DynamicEnums")) {
				ScriptableObjectUtility.CreateAsset<DynamicEnumDefinitions>("DynamicEnums", "Assets/Resources");
			}
		}

		[MenuItem("Tools/Dynamic Enums/Reload", priority = 1)]
		private static void reload() {
			if (DynamicEnumManager.Reload()) {
				EditorUtility.DisplayDialog("Dynamic Enums", "Dynamic Enums have been reloaded.", "Ok");
			} else {
				ScriptableObjectUtility.CreateAsset<DynamicEnumDefinitions>("DynamicEnums", "Assets/Resources");
			}
		}
	}
}