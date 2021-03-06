﻿using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	public static class ScriptableObjectUtility {
		// TODO Refactor these CreateAsset methods to avoid repeating code.

		/// <summary>
		/// Creates a new ScriptableObject via the default Save File panel
		/// </summary>
		public static ScriptableObject CreateAssetWithSavePrompt(Type type, string path) {
			path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", "New " + type.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);
			if (path == "") return null;
			var asset = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			EditorGUIUtility.PingObject(asset);
			return asset;
		}

		/// <summary>
		/// Creates a new ScriptableObject via the default Save File panel
		/// </summary>
		public static T CreateAssetWithSavePrompt<T>(string path) where T: ScriptableObject{
			path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", "New " + typeof(T).Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);
			if (path == "") return null;
			var asset = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			EditorGUIUtility.PingObject(asset);
			return asset;
		}

		/// <summary>
		/// This makes it easy to create, name and place unique new ScriptableObject asset files.
		/// </summary>
		public static void CreateAsset<T>(string name = null, string path = null, bool createPathIfNotExists = false) where T : ScriptableObject { // https://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
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

			if (!Directory.Exists(path)) {
				if (createPathIfNotExists) {
					Directory.CreateDirectory(path);
				}
				else {
					Debug.LogError($"Can't create the requested asset because the provided path doesn't exist.");
					return;
				}
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