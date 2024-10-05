using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CommonUtils.Editor {
	public class TaggedFinderWindow : EditorWindow {
		private static TaggedFinderWindow instance;

		private int selectedTagIndex;

		[MenuItem("Tools/Find objects with tag...")]
		private static void open() {
			if (!instance) {
				instance              = GetWindow<TaggedFinderWindow>();
				instance.titleContent = new GUIContent("Find objects with tag");
				instance.maxSize      = new Vector2(400f, 300f);
			}

			instance.Show();
		}

		private void OnGUI() {
			selectedTagIndex = EditorGUILayout.Popup("Tag", selectedTagIndex, InternalEditorUtility.tags);
			var tag = InternalEditorUtility.tags[selectedTagIndex];

			if (GUILayout.Button("Find with tag")) {
				var taggedObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g=>g.CompareTag(tag)).ToList();
				if (!taggedObjects.Any()) {
					Debug.Log($"No usages of tag \"{tag}\" were found in the build or resources folder.");
				}

				foreach (var tagged in taggedObjects) {
					Debug.Log($"{tagged.name} in scene {tagged.scene.name} has tag \"{tag}\"", tagged);
				}
			}
		}
	}
}