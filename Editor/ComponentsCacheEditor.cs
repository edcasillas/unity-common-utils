using System.Linq;
using CommonUtils.ComponentCaching;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	[CustomEditor(typeof(ComponentsCache))]
	public class ComponentsCacheEditor : UnityEditor.Editor {
		private bool fold = true;

		public override void OnInspectorGUI() {
			var cache = (ComponentsCache)target;

			if (Application.isPlaying) {
				var cachedObjects = cache.CachedObjects.ToArray();
				if (cachedObjects.Length > 0) {
					fold = EditorGUILayout.Foldout(fold, $"CachedObjects ({cachedObjects.Length})", true);
					if (fold) {
						EditorGUI.BeginDisabledGroup(true);
						for (var i = 0; i < cachedObjects.Length; i++) {
							EditorExtensions.ObjectField(i.ToString(), cachedObjects[i]);
						}

						EditorGUI.EndDisabledGroup();
					}
				} else {
					EditorGUILayout.LabelField("Cache is empty");
				}

				EditorUtility.SetDirty(target);
			}

			DrawDefaultInspector();
		}
	}
}