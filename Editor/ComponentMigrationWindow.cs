using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CommonUtils.Editor {
	public class ComponentMigrationWindow : EditorWindow {
		private MonoScript legacyScript;
		private MonoScript newScript;
		private Vector2 scrollPosition;
		private string statusMessage = "";

		[MenuItem("Tools/Component Migration...")]
		public static void ShowWindow() => GetWindow<ComponentMigrationWindow>("Component Migration");

		private void OnGUI() {
			EditorGUILayout.LabelField("Component Migration Utility", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			legacyScript =
				(MonoScript)EditorGUILayout.ObjectField("Legacy Script", legacyScript, typeof(MonoScript), false);
			newScript = (MonoScript)EditorGUILayout.ObjectField("New Script", newScript, typeof(MonoScript), false);

			EditorGUILayout.Space();

			GUI.enabled = legacyScript != null && newScript != null;
			if (GUILayout.Button("Migrate All", GUILayout.Height(30))) {
				MigrateAll();
			}

			GUI.enabled = true;

			EditorGUILayout.Space();
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			EditorGUILayout.TextArea(statusMessage, GUILayout.ExpandHeight(true));
			EditorGUILayout.EndScrollView();
		}

		private void MigrateAll() {
			if (!EditorUtility.DisplayDialog("Migrate Components",
					$"Replace all instances of '{legacyScript.name}' with '{newScript.name}'?",
					"Yes",
					"Cancel"))
				return;

			statusMessage = "Starting migration...\n";
			int total = 0;

			total += MigratePrefabs();
			total += MigrateScenes();

			AssetDatabase.SaveAssets();
			statusMessage += $"\nMigration complete. {total} components replaced.";
		}

		private int MigratePrefabs() {
			var prefabPaths = AssetDatabase.FindAssets("t:Prefab")
				.Select(AssetDatabase.GUIDToAssetPath)
				.ToArray();

			int count = 0;
			for (int i = 0; i < prefabPaths.Length; i++) {
				if (EditorUtility.DisplayCancelableProgressBar("Migrating Prefabs",
						prefabPaths[i],
						(float)i / prefabPaths.Length))
					break;

				var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPaths[i]);
				if (prefab != null) {
					int migrated = MigrateGameObject(prefab);
					if (migrated > 0) {
						EditorUtility.SetDirty(prefab);
						count += migrated;
						statusMessage += $"Prefab: {prefabPaths[i]} ({migrated})\n";
					}
				}
			}

			EditorUtility.ClearProgressBar();
			return count;
		}

		private int MigrateScenes() {
			var scenePaths = AssetDatabase.FindAssets("t:Scene")
				.Select(AssetDatabase.GUIDToAssetPath)
				.ToArray();

			int count = 0;
			for (int i = 0; i < scenePaths.Length; i++) {
				if (EditorUtility.DisplayCancelableProgressBar("Migrating Scenes",
						scenePaths[i],
						(float)i / scenePaths.Length))
					break;

				var scene = EditorSceneManager.OpenScene(scenePaths[i], OpenSceneMode.Additive);
				int migrated = 0;

				foreach (var root in scene.GetRootGameObjects())
					migrated += MigrateGameObject(root);

				if (migrated > 0) {
					EditorSceneManager.SaveScene(scene);
					count += migrated;
					statusMessage += $"Scene: {scenePaths[i]} ({migrated})\n";
				}

				EditorSceneManager.CloseScene(scene, true);
			}

			EditorUtility.ClearProgressBar();
			return count;
		}

		private int MigrateGameObject(GameObject go) {
			var legacyType = legacyScript.GetClass();
			var newType = newScript.GetClass();

			if (legacyType == null || newType == null || !typeof(MonoBehaviour).IsAssignableFrom(legacyType) ||
				!typeof(MonoBehaviour).IsAssignableFrom(newType))
				return 0;

			var components = go.GetComponentsInChildren(legacyType, true);
			int count = 0;

			foreach (Component comp in components) {
				var serialized = new SerializedObject(comp);
				var gameObject = comp.gameObject;
				var wasEnabled = (comp as MonoBehaviour)?.enabled ?? true;

				var newComp = gameObject.AddComponent(newType) as MonoBehaviour;
				CopySerializedFields(serialized, new SerializedObject(newComp));

				if (newComp != null)
					newComp.enabled = wasEnabled;

				DestroyImmediate(comp, true);
				count++;
			}

			return count;
		}

		private void CopySerializedFields(SerializedObject source, SerializedObject dest) {
			var prop = source.GetIterator();
			while (prop.NextVisible(true)) {
				if (prop.name == "m_Script")
					continue;

				var destProp = dest.FindProperty(prop.name);
				if (destProp != null)
					dest.CopyFromSerializedProperty(prop);
			}

			dest.ApplyModifiedProperties();
		}
	}
}