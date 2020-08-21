using System.Linq;
using CommonUtils.Effects.ShakingTransform;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(ShakingTransformController))]
    public class ShakingTransformControllerEditor : UnityEditor.Editor {
		private ShakingTransformController shakingTransformController;
		private float stress = 1f;
		private int selectedPresetIndex = 0;
		private bool testCustomPreset = false;
		private ShakingTransformPreset customPreset;

		private void OnEnable() => shakingTransformController = (ShakingTransformController)target;

		public override void OnInspectorGUI() {
			if (!Application.isPlaying) {
				var helpBoxType = shakingTransformController.transform.parent ? MessageType.Info : MessageType.Warning;
				EditorGUILayout.HelpBox(
					"Please make sure this Game Object is a children of another one to preserve the original position and rotation when and after shaking.",
					helpBoxType);

				if (GUILayout.Button("Create parent")) {
					GameObject parent;
					var rectTransform = shakingTransformController.transform as RectTransform;
					if (rectTransform) {
						parent = new GameObject("ShakingTransformParent", typeof(RectTransform));
						var parentRectTransform = parent.GetComponent<RectTransform>();
						parentRectTransform.anchorMin = rectTransform.anchorMin;
						parentRectTransform.anchorMax = rectTransform.anchorMax;
						parentRectTransform.pivot = rectTransform.pivot;
						parentRectTransform.anchoredPosition = rectTransform.anchoredPosition;
						parentRectTransform.sizeDelta = rectTransform.sizeDelta;
						parentRectTransform.offsetMin = rectTransform.offsetMin;
						parentRectTransform.offsetMax = rectTransform.offsetMax;
					} else {
						parent = new GameObject("ShakingTransformParent");
						parent.transform.position = shakingTransformController.transform.position;
						parent.transform.rotation = shakingTransformController.transform.rotation;
					}
					Undo.RegisterCreatedObjectUndo(parent, "create ShakingTransformParent");
					if (shakingTransformController.transform.parent)
						Undo.SetTransformParent(parent.transform,
							shakingTransformController.transform.parent,
							"set new parent");
					Undo.SetTransformParent(shakingTransformController.transform, parent.transform, "set new parent");

					if (rectTransform) {
						Undo.RecordObject(rectTransform, $"resize {rectTransform.name}");
						rectTransform.anchorMin = Vector2.zero;
						rectTransform.anchorMax = Vector2.one;
						rectTransform.pivot = new Vector2(0.5f,0.5f);
						rectTransform.anchoredPosition = Vector2.zero;
						rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
					}
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
					EditorExtensions.ReadOnlyLabelField("Current Preset", shakingTransformController.CurrentPreset.name);
				}, "Debug");

				EditorExtensions.BoxGroup(() => {
					var presetNames = shakingTransformController.Presets.Keys.ToArray();
					if (!testCustomPreset) {
						if(presetNames.Length > 0) selectedPresetIndex = EditorGUILayout.Popup("Preset", Mathf.Clamp(selectedPresetIndex, 0, presetNames.Length - 1), presetNames);
						else EditorGUILayout.HelpBox("No presets have been defined.", MessageType.Warning);
					}
					//if(GUILayout.Button("Reload Presets")) shakingTransformController.ReloadPresets();

					testCustomPreset = EditorExtensions.Collapse(testCustomPreset,
						"Use custom preset",
						() => {
							if (customPreset == null) {
								customPreset = new ShakingTransformPreset();
								customPreset.name = "Custom test preset";
							}
							customPreset.Intensity = EditorGUILayout.Vector3Field(new GUIContent("Intensity","Defines the maximum translation at each axis."), customPreset.Intensity);
							customPreset.AngularIntensity = EditorGUILayout.Vector3Field(new GUIContent("Angular Intensity","Defines the maximum rotation at each axis."), customPreset.AngularIntensity);
							customPreset.Frequency = EditorGUILayout.FloatField(new GUIContent("Frequency","How fast is the shake?"), customPreset.Frequency);
							customPreset.RecoverySpeed = EditorGUILayout.FloatField(new GUIContent("Recovery Speed","How fast will the shaking end?"), customPreset.RecoverySpeed);
							customPreset.Magnitude = EditorGUILayout.FloatField(new GUIContent("Magnitude","Multiplier to control the smooth falloff of the shake."), customPreset.Magnitude);
						});

					stress = EditorGUILayout.FloatField("Stress to apply", stress);

					if (GUILayout.Button("Apply stress")) {
						if(!testCustomPreset) shakingTransformController.InduceStress(stress, presetNames.Length > 0 ? presetNames[selectedPresetIndex] : null);
						else shakingTransformController.InduceStress(customPreset, stress);
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