using CommonUtils.UI;
using CommonUtils.WebResources;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
	[CustomEditor(typeof(AbstractWebResourceComponent), true)]
	public class WebResourceComponentEditor : UnityEditor.Editor {
		private AbstractWebResourceComponent webResourceComponent;

		private void OnEnable() => webResourceComponent = (AbstractWebResourceComponent)target;

		public override void OnInspectorGUI() {
			if (Application.isPlaying) {
				EditorExtensions.BoxGroup(() => {
					EditorExtensions.ReadOnlyLabelField("Status", webResourceComponent.Status);

					if (GUILayout.Button("Load")) {
						webResourceComponent.Load();
					}
				});
				EditorGUILayout.Space();
				EditorUtility.SetDirty(target);
			}

			DrawDefaultInspector();
		}
	}
}
