using CommonUtils.Effects;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(ShakingTransformController))]
    public class ShakingTransformControllerEditor : UnityEditor.Editor {
		private float stress = 1f;

		private ShakingTransformController shakingTransformController;

		private void OnEnable() => shakingTransformController = (ShakingTransformController)target;

		public override void OnInspectorGUI() {
			if (!Application.isPlaying) {
				var helpBoxType = shakingTransformController.transform.parent ? MessageType.Info : MessageType.Warning;
				EditorGUILayout.HelpBox(
					"Please make sure this Game Object is a children of another one to preserve the original position and rotation when and after shaking.",
					helpBoxType);

				if (GUILayout.Button("Create parent")) {
					var parent = new GameObject("ShakingTransformParent");
					parent.transform.position = shakingTransformController.transform.position;
					parent.transform.rotation = shakingTransformController.transform.rotation;
					Undo.RegisterCreatedObjectUndo(parent, "create ShakingTransformParent");
					if (shakingTransformController.transform.parent)
						Undo.SetTransformParent(parent.transform,
							shakingTransformController.transform.parent,
							"set new parent");
					Undo.SetTransformParent(shakingTransformController.transform, parent.transform, "set new parent");
				}

				EditorGUILayout.Space();
			}

			if (!Application.isPlaying) {
                EditorGUILayout.HelpBox("Enter play mode to test this component.", MessageType.Info);
            } else {
				EditorExtensions.BoxGroup(() => {
					EditorExtensions.ReadOnlyLabelField("Seed", shakingTransformController.Seed);
					EditorExtensions.ReadOnlyLabelField("Trauma", shakingTransformController.Trauma);
					EditorExtensions.Disabled(() => {
						EditorGUILayout.Toggle("IsShaking", shakingTransformController.IsShaking);});
				}, "Debug");

				EditorExtensions.BoxGroup(() => {
					stress = EditorGUILayout.FloatField("Stress to apply", stress);

					if (GUILayout.Button("Apply stress")) {
						shakingTransformController.InduceStress(stress);
					}

					if (shakingTransformController.Trauma > 0) {
						if (shakingTransformController.IsPaused) {
							if (GUILayout.Button("Resume")) {
								shakingTransformController.Resume();
							}
						} else {
							if (GUILayout.Button("Pause")) {
								shakingTransformController.Pause();
							}
						}

						if (GUILayout.Button("Stop")) {
							shakingTransformController.Stop();
						}
					}
				}, "Shake tester");
			}

			EditorGUILayout.Space();

			DrawDefaultInspector();
		}
    }
}