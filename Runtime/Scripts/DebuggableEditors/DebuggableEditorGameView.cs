using CommonUtils.Extensions;
using CommonUtils.Inspector.HelpBox;
using CommonUtils.LegacyGUI;
using GUIMaquetter;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.DebuggableEditors {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
	[ExecuteInEditMode]
	public class DebuggableEditorGameView : MonoBehaviour {
		private const float DEFAULT_HEIGHT_PCT = 0.3f;

		[HelpBox("Please note this component will only work in the Unity Editor and Development Builds.", HelpBoxMessageType.Warning)]
		[SerializeField] [Range(0f, 1f)] private float positionX;
		[SerializeField] [Range(0f, 1f)] private float positionY;
		[SerializeField] [Range(0f, 1f)] private float width = 1f;
		[SerializeField] private Color backgroundColor = Color.black;
		[SerializeField] private Color fontColor = Color.white;
		[SerializeField] private int fontSize = 15;
		[SerializeField] private MonoBehaviour debuggableComponent;
		[SerializeField] private bool debugAllPropertiesAndMethods;

		private Box box;

		private void OnGUI() {
			GUICoords.Instance.AdjustGUIMatrix();

			GUI.color = fontColor;
			GUI.backgroundColor = backgroundColor;
			GUI.skin.box.fontSize = GUI.skin.label.fontSize = GUI.skin.button.fontSize = fontSize;

			var componentName = "";
			ICollection<ReflectedProperty> reflectedProperties = null;
			ICollection<ReflectedMethod> reflectedMethods = null;
			if (debuggableComponent) {
				var debuggableComponentType = debuggableComponent.GetType();
				componentName = debuggableComponentType.Name.PascalToTitleCase();
				reflectedProperties = debuggableComponentType.GetDebuggableProperties(debugAllPropertiesAndMethods);
				reflectedMethods = debuggableComponentType.GetDebuggableMethods(debugAllPropertiesAndMethods);
			}

			var actualRect = GUICoords.Instance.GetRect(positionX, positionY, width, DEFAULT_HEIGHT_PCT);

			var rowSize = GUI.skin.box.CalcSize(new GUIContent(componentName));

			var requiredRows = (reflectedProperties?.Count ?? 0) + (reflectedMethods?.Count ?? 0);
			if (requiredRows > 0) {
				actualRect.height = requiredRows * rowSize.y;
			}

			box ??= new Box();
			box.Name = componentName;
			box.Content = new GUIContent(componentName);
			box.Rect = actualRect;

			box.Draw(Event.current);

			var i = 1;
			if (reflectedProperties != null) {
				foreach (var reflectedProperty in reflectedProperties) {
					var val = "-";
					if (Application.isPlaying) {
						val = reflectedProperty.GetValue(debuggableComponent)?.ToString() ?? "<null>";
					}

					GUI.Label(new Rect(actualRect.x, actualRect.y + (rowSize.y * i), actualRect.width, rowSize.y),
						$"{reflectedProperty.DisplayName}: {val}");
					i++;
				}
			}

			if (reflectedMethods != null) {
				foreach (var reflectedMethod in reflectedMethods) {
					if (GUI.Button(new Rect(actualRect.x, actualRect.y + (rowSize.y * i), actualRect.width, rowSize.y),
							reflectedMethod.DisplayName)) {
						reflectedMethod.Invoke(debuggableComponent);
					}

					i++;
				}
			}
		}

		private void renderMethod(object instance, ReflectedMethod reflectedMethod) {
			/*if (reflectedMethod.HasOutParameters) {
				EditorGUILayout.HelpBox(
					$"Method \"{reflectedMethod.RealName}\" has at least one out parameter. This is not yet supported by the Debuggable Editor.",
					MessageType.Warning);
				return;
			}*/

			/*if (reflectedMethod.HasParameters) {
				GUILayout.BeginVertical(reflectedMethod.DisplayName, "box");
				EditorGUILayout.Space(20);
				reflectedMethod.RenderDebugInfoIfAny();
				for (int i = 0; i < reflectedMethod.ParamInfo.Length; i++) {
					reflectedMethod.Arguments[i] = EditorExtensions.RenderField(
						reflectedMethod.ParamInfo[i].ParameterType,
						reflectedMethod.ParamInfo[i].Name.PascalToTitleCase(),
						reflectedMethod.Arguments[i]);
				}
			} else {
				GUILayout.BeginVertical("box");
				reflectedMethod.RenderDebugInfoIfAny();
			}*/

			if (!reflectedMethod.IsAwaitable || !reflectedMethod.HasBeenCalled || reflectedMethod.FinishedExecuting) {
				if (GUILayout.Button(reflectedMethod.HasParameters ? "Invoke" : reflectedMethod.DisplayName)) {
					reflectedMethod.Invoke(instance);
				}
			}

			/*if (reflectedMethod.HasBeenCalled && reflectedMethod.HasReturnValue) {
				if (reflectedMethod.FinishedExecuting) {
					if (reflectedMethod.IsAwaitable) {
						EditorGUILayout.HelpBox(
							$"Async task took {reflectedMethod.StopWatch.Elapsed.TotalSeconds} seconds.",
							MessageType.Info);
					}

					var fold = reflectedMethod.Fold;
					if (TryRenderEnumerableField(reflectedMethod.ReturnValue, "Result", ref fold)) {
						reflectedMethod.Fold = fold;
					} else {
						EditorExtensions.RenderField(reflectedMethod.Type, "Result", reflectedMethod.ReturnValue);
					}
				} else {
					EditorGUILayout.HelpBox($"Executing for {reflectedMethod.StopWatch.Elapsed.TotalSeconds} seconds.",
						MessageType.Info);
				}
			}

			GUILayout.EndVertical();*/
		}
	}
#endif
}