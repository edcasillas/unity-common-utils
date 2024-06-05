using CommonUtils.Extensions;
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
		private const string EDITOR_PREF_KEY_USE_CODE_STRIP = "AssetCleaner.UseCodeStrip";
		private const string EDITOR_PREF_KEY_USE_SAVE_EDITOR_EXTENSIONS = "AssetCleaner.SaveEditorExtension";
		#endregion

		#region Properties
		private static bool useCodeStrip {
			get => EditorPrefs.GetBool(EDITOR_PREF_KEY_USE_CODE_STRIP, true);
			set => EditorPrefs.SetBool(EDITOR_PREF_KEY_USE_CODE_STRIP, value);
		}

		private static bool saveEditorExtensions {
			get => EditorPrefs.GetBool(EDITOR_PREF_KEY_USE_SAVE_EDITOR_EXTENSIONS, true);
			set => EditorPrefs.SetBool(EDITOR_PREF_KEY_USE_SAVE_EDITOR_EXTENSIONS, value);
		}
		#endregion

		#region Fields
		private string searchText = string.Empty;
		private bool selectAll = true;
		private AssetCollector collection = new();
		private List<DeleteAsset> deleteAssets = new();
		private Vector2 scroll;
		#endregion

		[MenuItem("Tools/Asset Cleaner...", false, 52)]
		private static void openAssetCleaner () {
			var window = CreateInstance<AssetCleanerWindow>();
			window.titleContent = new GUIContent("Asset Cleaner");
			window.refresh();
			window.Show ();
		}

		private void refresh() {
			deleteAssets = new List<DeleteAsset>();
			selectAll = true;
			collection.useCodeStrip = useCodeStrip;
			collection.saveEditorExtensions = saveEditorExtensions;
			collection.Collection();
			copyDeleteFileList(collection.deleteFileList);
		}

		#region Unity Lifecycle
		private void OnGUI() {
			using (new EditorGUILayout.HorizontalScope("box")) {
				useCodeStrip = GUILayout.Toggle(useCodeStrip, "Use Code Strip");
				saveEditorExtensions = GUILayout.Toggle(saveEditorExtensions, "Save Editor Extensions");

				if (GUILayout.Button("Refresh")) { refresh(); }
			}

			drawFileCountLabelAndRemoveButton(deleteAssets);

			if(deleteAssets.IsNullOrEmpty()) return;

			using (new EditorGUILayout.HorizontalScope("box")) {
				EditorGUILayout.LabelField("Search");
				searchText = EditorGUILayout.TextField(searchText);
			}


			var newSelectAll = GUILayout.Toggle(selectAll, "Select All");

			if (newSelectAll != selectAll) {
				changeSelectionAll(deleteAssets, newSelectAll);
				selectAll = newSelectAll;
				return;
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
						asset.IsSelected = EditorGUILayout.Toggle(asset.IsSelected, GUILayout.Width(20));
						selectAll = selectAll && asset.IsSelected;
						var icon = AssetDatabase.GetCachedIcon(asset.path);
						GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));

						if (GUILayout.Button(assetPath, EditorStyles.largeLabel)) {
							Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(asset.path);
						}
					}
				}
			}
		}
		#endregion

		private void drawFileCountLabelAndRemoveButton(IEnumerable<DeleteAsset> allAssetsFound) {
			using (new EditorGUILayout.HorizontalScope("box")) {
				var allFiles = allAssetsFound.Where(item => !string.IsNullOrWhiteSpace(item.path));
				var fileCount = allFiles.Count();
				var selectedFiles = allAssetsFound.Count(item => item.IsSelected);

				var labelMessage = $"{(fileCount > 0 ? fileCount.ToString() : "No")} unused assets found.";
				if (fileCount > 0) labelMessage += $" {selectedFiles} selected.";

				EditorGUILayout.LabelField(labelMessage);

				EditorGUI.BeginDisabledGroup(selectedFiles == 0);
				if (GUILayout.Button("Remove", GUILayout.Width(120), GUILayout.Height(40))) {
					// RemoveFiles();
					refresh();
				}
				EditorGUI.EndDisabledGroup();
			}
		}

		private static void changeSelectionAll(IEnumerable<DeleteAsset> assets, bool select) {
			foreach (var asset in assets) {
				asset.IsSelected = select;
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

		private void removeFiles () {
			try {
				var exportDirectry = "BackupUnusedAssets";
				Directory.CreateDirectory (exportDirectry);
				var files = deleteAssets.Where (item => item.IsSelected).Select (item => item.path).ToArray ();
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
			catch( Exception e ){
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
			public bool IsSelected = true;
			public string path;
		}
	}
}
