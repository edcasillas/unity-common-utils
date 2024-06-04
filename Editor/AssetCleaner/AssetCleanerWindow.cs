using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Editor.AssetCleaner
{
	public class AssetCleanerWindow : EditorWindow {
		#region Constants
		public const string EDITOR_PREF_KEY_USE_CODE_STRIP = "AssetCleaner.UseCodeStrip";
		public const string EDITOR_PREF_KEY_USE_SAVE_EDITOR_EXTENSIOnS = "AssetCleaner.SaveEditorExtension";
		#endregion

		#region Properties
		internal static bool UseCodeStrip {
			get => EditorPrefs.GetBool(EDITOR_PREF_KEY_USE_CODE_STRIP, true);
			set => EditorPrefs.SetBool(EDITOR_PREF_KEY_USE_CODE_STRIP, value);
		}

		internal static bool SaveEditorExtensions {
			get => EditorPrefs.GetBool(EDITOR_PREF_KEY_USE_CODE_STRIP, true);
			set => EditorPrefs.SetBool(EDITOR_PREF_KEY_USE_CODE_STRIP, value);
		}
		#endregion

		private string searchText = string.Empty;

		AssetCollector collection = new();
		private List<DeleteAsset> deleteAssets = new();
		Vector2 scroll;

		[MenuItem("Assets/Delete Unused Assets/only resource", false, 50)]
		static void InitWithoutCode ()
		{
			var window = CreateInstance<AssetCleanerWindow> ();
			window.collection.useCodeStrip = false;
			window.collection.Collection ();
			window.copyDeleteFileList (window.collection.deleteFileList);

			window.Show ();
		}

		[MenuItem("Assets/Delete Unused Assets/unused by editor", false, 51)]
		static void InitWithout ()
		{
			var window = CreateInstance<AssetCleanerWindow> ();
			window.collection.Collection ();
			window.copyDeleteFileList (window.collection.deleteFileList);

			window.Show ();
		}

		[MenuItem("Tools/Asset Cleaner...", false, 52)]
		private static void openAssetCleaner () {
			var window = CreateInstance<AssetCleanerWindow>();
			window.titleContent = new GUIContent("Asset Cleaner");
			window.refresh(saveEditorExtensions: false);
			window.Show ();
		}

		private void refresh(bool useCodeStrip = true, bool saveEditorExtensions = true) {
			deleteAssets = new List<DeleteAsset>();
			collection.useCodeStrip = useCodeStrip;
			collection.saveEditorExtensions = saveEditorExtensions;
			collection.Collection();
			copyDeleteFileList(collection.deleteFileList);
		}

		private void OnGUI() {
			using (new EditorGUILayout.HorizontalScope("box")) {
				UseCodeStrip = GUILayout.Toggle(UseCodeStrip, "Use Code Strip");
				SaveEditorExtensions = GUILayout.Toggle(SaveEditorExtensions, "Save Editor Extensions");

				if (GUILayout.Button("Refresh")) {
					refresh(saveEditorExtensions: false);
				}
			}

			using (new EditorGUILayout.HorizontalScope("box")) {
				var fileCount = deleteAssets.Count(item => item.isDelete && !string.IsNullOrWhiteSpace(item.path));
				if (fileCount > 0) {
					EditorGUILayout.LabelField($"{fileCount} unused assets.");

					if (GUILayout.Button("Remove", GUILayout.Width(120), GUILayout.Height(40)) && deleteAssets.Count != 0) {
						// RemoveFiles();
						Close();
					}
				} else {
					EditorGUILayout.LabelField("No unused assets were found.");
				}
			}

			using (new EditorGUILayout.HorizontalScope("box")) {
				EditorGUILayout.LabelField("Search");
				searchText = EditorGUILayout.TextField(searchText);
			}

			using (var scrollScope = new EditorGUILayout.ScrollViewScope(scroll)) {
				scroll = scrollScope.scrollPosition;
				foreach (var asset in deleteAssets) {
					if (string.IsNullOrEmpty(asset.path)) {
						continue;
					}
					var assetPath = asset.path[(asset.path.StartsWith("Assets/") ? 7 : 0)..];
					if(!assetPath.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)) continue;

					using (new EditorGUILayout.HorizontalScope()) {
						asset.isDelete = EditorGUILayout.Toggle(asset.isDelete, GUILayout.Width(20));
						var icon = AssetDatabase.GetCachedIcon(asset.path);
						GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));

						if (GUILayout.Button(assetPath, EditorStyles.largeLabel)) {
							Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(asset.path);
						}
					}
				}
			}
		}

		static void CleanDir() {
			RemoveEmptyDirectry("Assets");
			AssetDatabase.Refresh();
		}

		private void copyDeleteFileList(IEnumerable<string> deleteFileList) {
			foreach (var asset in deleteFileList) {
				var filePath = AssetDatabase.GUIDToAssetPath(asset);
				if (!string.IsNullOrEmpty(filePath)) {
					deleteAssets.Add(new DeleteAsset() { path = filePath });
				}
			}
		}

		void RemoveFiles () {
			try {
				var exportDirectry = "BackupUnusedAssets";
				Directory.CreateDirectory (exportDirectry);
				var files = deleteAssets.Where (item => item.isDelete).Select (item => item.path).ToArray ();
				var backupPackageName = exportDirectry + "/package" + System.DateTime.Now.ToString ("yyyyMMddHHmmss") + ".unitypackage";
				EditorUtility.DisplayProgressBar ("export package", backupPackageName, 0);
				AssetDatabase.ExportPackage (files, backupPackageName);

				int i = 0;
				int length = deleteAssets.Count;

				foreach (var assetPath in files) {
					i++;
					EditorUtility.DisplayProgressBar ("delete unused assets", assetPath, (float)i / length);
					AssetDatabase.DeleteAsset (assetPath);
				}

				EditorUtility.DisplayProgressBar ("clean directory", "", 1);
				foreach (var dir in Directory.GetDirectories("Assets")) {
					RemoveEmptyDirectry (dir);
				}

				System.Diagnostics.Process.Start (exportDirectry);

				AssetDatabase.Refresh ();
			}
			catch( System.Exception e ){
				Debug.Log(e.Message);
			}finally {
				EditorUtility.ClearProgressBar ();
			}
		}

		static void RemoveEmptyDirectry (string path)
		{
			var dirs = Directory.GetDirectories (path);
			foreach (var dir in dirs) {
				RemoveEmptyDirectry (dir);
			}

			var files = Directory.GetFiles (path, "*", SearchOption.TopDirectoryOnly).Where (item => Path.GetExtension (item) != ".meta");
			if (files.Count () == 0 && Directory.GetDirectories (path).Count () == 0) {
				var metaFile = AssetDatabase.GetTextMetaFilePathFromAssetPath(path);
				UnityEditor.FileUtil.DeleteFileOrDirectory (path);
				UnityEditor.FileUtil.DeleteFileOrDirectory (metaFile);
			}
		}

		class DeleteAsset
		{
			public bool isDelete = true;
			public string path;
		}
	}
}
