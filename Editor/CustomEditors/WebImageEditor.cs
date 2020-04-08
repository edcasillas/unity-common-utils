using CommonUtils.UI;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
	[CustomEditor(typeof(WebImage))]
	public class WebImageEditor : UnityEditor.Editor {
		private WebImage webImage;

		private void OnEnable() => webImage = (WebImage)target;

		public override void OnInspectorGUI() {
			if (Application.isPlaying) {
				EditorExtensions.BoxGroup(() => {
					EditorExtensions.ReadOnlyLabelField("Status", webImage.Status);
				});
				EditorGUILayout.Space();
				EditorUtility.SetDirty(target);
			}

			DrawDefaultInspector();
		}
	}
}
