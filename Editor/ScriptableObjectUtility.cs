using System.IO;
using CommonUtils.Extensions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	public static class ScriptableObjectUtility {
		/// <summary>
		/// This makes it easy to create, name and place unique new ScriptableObject asset files.
		/// </summary>
		public static void CreateAsset<T>(string name = null, string path = null) where T : ScriptableObject { // https://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
			T asset = ScriptableObject.CreateInstance<T>();

			if (string.IsNullOrWhiteSpace(path)) {
				path = AssetDatabase.GetAssetPath(Selection.activeObject);
				if (path == "") {
					path = "Assets";
				} else if (Path.GetExtension(path) != "") {
					path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
				}
			}

			if (string.IsNullOrWhiteSpace(name)) {
				name = "New " + typeof(T);
			}

			var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");

			AssetDatabase.CreateAsset(asset, assetPathAndName);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
	}
}