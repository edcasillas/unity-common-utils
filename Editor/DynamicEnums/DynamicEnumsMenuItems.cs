using CommonUtils.DynamicEnums;
using UnityEditor;

namespace CommonUtils.Editor.DynamicEnums {
	public static class DynamicEnumsMenuItems {
		[MenuItem(MenuItems.MenuPathPrefix + "Dynamic Enums...", priority = 0)]
		private static void configure() {
			if (!EditorUtils.HighlightAssetOfType<DynamicEnumDefinitions>("DynamicEnums")) {
				ScriptableObjectUtility.CreateAsset<DynamicEnumDefinitions>("DynamicEnums", "Assets/Resources", true);
			}
		}
	}
}