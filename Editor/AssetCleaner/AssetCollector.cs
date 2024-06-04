using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.AssetCleaner
{
	public class AssetCollector
	{
		public List<string> deleteFileList = new List<string> ();
		ClassReferenceCollection classCollection = new ClassReferenceCollection ();
		ShaderReferenceCollection shaderCollection = new ShaderReferenceCollection ();

		public bool useCodeStrip = true;
		public bool saveEditorExtensions = true;

		public void Collection ()
		{
			try {
				deleteFileList.Clear ();

				if (useCodeStrip) classCollection.Collection();
				shaderCollection.Collection ();

				// Find assets
				var files = Directory.GetFiles ("Assets", "*.*", SearchOption.AllDirectories)
					.Where (item => Path.GetExtension (item) != ".meta")
					.Where (item => Path.GetExtension (item) != ".js")
					.Where (item => Path.GetExtension (item) != ".dll")
					.Where (item => Regex.IsMatch (item, "[\\/\\\\]Gizmos[\\/\\\\]") == false)
					.Where (item => Regex.IsMatch (item, "[\\/\\\\]Plugins[\\/\\\\]Android[\\/\\\\]") == false)
					.Where (item => Regex.IsMatch (item, "[\\/\\\\]Plugins[\\/\\\\]iOS[\\/\\\\]") == false)
					.Where (item => Regex.IsMatch (item, "[\\/\\\\]Resources[\\/\\\\]") == false);

				if( useCodeStrip == false ){
					files = files.Where( item => Path.GetExtension(item) != ".cs");
				}

				foreach (var path in files) {
					var guid = AssetDatabase.AssetPathToGUID (path);
					deleteFileList.Add (guid);
				}
				EditorUtility.DisplayProgressBar ("checking", "collection all files", 0.2f);
				UnregistReferenceFromResources();

				EditorUtility.DisplayProgressBar ("checking", "check reference from resources", 0.4f);
				UnregistReferenceFromScenes();

				EditorUtility.DisplayProgressBar ("checking", "check reference from scenes", 0.6f);
				if( saveEditorExtensions ){
					UnregistEditorCodes();
				}
			} finally {
				EditorUtility.ClearProgressBar ();
			}
		}

		private void UnregistReferenceFromResources()
		{
			var resourcesFiles = Directory.GetFiles ("Assets", "*.*", SearchOption.AllDirectories)
				.Where (item => Regex.IsMatch (item, "[\\/\\\\]Resources[\\/\\\\]") == true)
					.Where (item => Path.GetExtension (item) != ".meta")
					.ToArray ();
			foreach (var path in AssetDatabase.GetDependencies (resourcesFiles)) {
				unregistFromDelteList (AssetDatabase.AssetPathToGUID(path));
			}
		}

		void UnregistReferenceFromScenes()
		{
			// Exclude objects that reference from scenes.
			var scenes = EditorBuildSettings.scenes
				.Where (item => item.enabled == true)
					.Select (item => item.path)
					.ToArray ();
			foreach (var path in AssetDatabase.GetDependencies (scenes)) {
				if( saveEditorExtensions == false ){
					Debug.Log(path);
				}
				unregistFromDelteList (AssetDatabase.AssetPathToGUID(path));
			}
		}

		void UnregistEditorCodes()
		{
			// Exclude objects that reference from Editor API
			var editorcodes = Directory.GetFiles ("Assets", "*.cs", SearchOption.AllDirectories)
				.Where (item => Regex.IsMatch (item, "[\\/\\\\]Editor[\\/\\\\]") == true)
					.ToArray ();

			var undeleteClassList = classCollection.codeFileList
				.Where (codefile => codefile.Value.Any( guid => deleteFileList.Contains(guid)) == false)
					.Select( item => item.Key );

			EditorUtility.DisplayProgressBar ("checking", "check reference from editor codes", 0.8f);

			foreach (var path in editorcodes) {
				var code = File.ReadAllText (path);
				code = Regex.Replace(code, "//.*[\\n\\r]", "");
				code = Regex.Replace(code, "/\\*.*[\\n\\r]\\*/", "");
				if (Regex.IsMatch (code, "(\\[MenuItem|AssetPostprocessor|EditorWindow)")) {
					unregistFromDelteList ( AssetDatabase.AssetPathToGUID(path));
					continue;
				}

				foreach (var undeleteClass in undeleteClassList) {
					if (Regex.IsMatch (code, string.Format ("\\[CustomEditor.*\\(\\s*{0}\\s*\\).*\\]", undeleteClass.Name))) {
						unregistFromDelteList (path);
						continue;
					}
				}
			}
		}

		private void unregistFromDelteList (string guid) {
			if (!deleteFileList.Contains (guid)) return;
			deleteFileList.Remove (guid);

			if (classCollection.references.ContainsKey (guid)) {
				foreach (var type in classCollection.references[guid]) {
					var codePaths = classCollection.codeFileList [type];
					foreach( var codePath in codePaths){
						unregistFromDelteList (codePath);
					}
				}
			}

			if (shaderCollection.shaderFileList.ContainsValue (guid)) {
				var shader = shaderCollection.shaderFileList.First (item => item.Value == guid);
				var shaderAssets = shaderCollection.shaderReferenceList [shader.Key];
				foreach (var shaderPath in shaderAssets) {
					unregistFromDelteList (shaderPath);
				}
			}
		}
	}
}
