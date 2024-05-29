using CommonUtils.DebuggableEditors;
using CommonUtils.Extensions;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.DebuggableEditors {
	internal static class DebuggableEditorUtils {
		internal static void RenderDebuggableMembers<T>(this DebuggableComponentData debuggableComponentData, T instance) where T : MonoBehaviour {
			if(!instance) return;

			foreach (var prop in debuggableComponentData.DebuggableProperties) {
				RenderProperty(instance, prop);
			}

			foreach (var method in debuggableComponentData.DebuggableMethods) {
				RenderMethod(instance, method);
			}
		}

		private static void RenderProperty(object instance, ReflectedProperty reflectedProperty) {
			if (!reflectedProperty.HasPublicGetter) {
				EditorGUILayout.HelpBox(
					$"Property {reflectedProperty.RealName} has been tagged with {nameof(ShowInInspectorAttribute)} but the value cannot be read because it doesn't have a setter.",
					MessageType.Error);
				return;
			}

			if (reflectedProperty.SetterIsEnabled) {
				if (!reflectedProperty.HasPublicSetter) {
					EditorGUILayout.HelpBox(
						$"Property {reflectedProperty.RealName} has been tagged with {nameof(ShowInInspectorAttribute)} with EnableSetter "
					  + $"= true, but a public setter has not been defined, the property is therefore read-only.",
						MessageType.Error);
				}
			}

			object oldValue;
			try {
				oldValue = reflectedProperty.GetValue(instance);
			} catch (Exception ex) {
				var exceptionMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				EditorGUILayout.HelpBox(
					$"An exception occurred while calling the getter of property \"{reflectedProperty.RealName}\": {exceptionMessage}",
					MessageType.Error);
				return;
			}

			reflectedProperty.RenderDebugInfoIfAny();

			var newValue = RenderEditorField(reflectedProperty, oldValue);

			if (reflectedProperty.SetterIsEnabled && reflectedProperty.HasPublicSetter && !oldValue.Equals(newValue)) {
				reflectedProperty.SetValue(instance, newValue);
			}
		}

		private static void RenderMethod(object instance, ReflectedMethod reflectedMethod) {
			if (reflectedMethod.HasOutParameters) {
				EditorGUILayout.HelpBox(
					$"Method \"{reflectedMethod.RealName}\" has at least one out parameter. This is not yet supported by the Debuggable Editor.",
					MessageType.Warning);
				return;
			}

			if (reflectedMethod.HasParameters) {
				GUILayout.BeginVertical(reflectedMethod.DisplayName, "box");
				EditorGUILayout.Space(20);
				reflectedMethod.RenderDebugInfoIfAny();
				for (var i = 0; i < reflectedMethod.ParamInfo.Length; i++) {
					reflectedMethod.Arguments[i] = EditorExtensions.RenderField(
						reflectedMethod.ParamInfo[i].ParameterType,
						reflectedMethod.ParamInfo[i].Name.PascalToTitleCase(),
						reflectedMethod.Arguments[i]);
				}
			} else {
				GUILayout.BeginVertical("box");
				reflectedMethod.RenderDebugInfoIfAny();
			}

			if (!reflectedMethod.IsAwaitable || !reflectedMethod.HasBeenCalled || reflectedMethod.FinishedExecuting) {
				if (GUILayout.Button(reflectedMethod.HasParameters ? "Invoke" : reflectedMethod.DisplayName)) {
					reflectedMethod.Invoke(instance);
				}
			}

			if (reflectedMethod.HasBeenCalled) {
				if (reflectedMethod.FinishedExecuting) {
					if (reflectedMethod.IsAwaitable) {
						EditorGUILayout.HelpBox(
							$"Async task took {reflectedMethod.StopWatch.Elapsed.TotalSeconds} seconds.",
							MessageType.Info);
					}

					if (reflectedMethod.Exception != null) {
						EditorGUILayout.HelpBox(
							$"{reflectedMethod.Exception.GetType().Name}: {reflectedMethod.Exception.Message}",
							MessageType.Error);
					} else if (reflectedMethod.HasReturnValue) {
						var fold = reflectedMethod.Fold;
						if (TryRenderEnumerableField(reflectedMethod.ReturnValue, "Result", ref fold)) {
							reflectedMethod.Fold = fold;
						} else {
							EditorExtensions.RenderField(reflectedMethod.Type, "Result", reflectedMethod.ReturnValue);
						}
					}
				} else {
					EditorGUILayout.HelpBox($"Executing for {reflectedMethod.StopWatch.Elapsed.TotalSeconds} seconds.",
						MessageType.Info);
				}
			}

			GUILayout.EndVertical();
		}

		private static object RenderEditorField(ReflectedProperty reflectedProperty, object value) {
			if (reflectedProperty.UseTextArea) {
				EditorGUILayout.LabelField(new GUIContent(reflectedProperty.DisplayName, reflectedProperty.HelpText));
				return EditorGUILayout.TextArea(value.ToString(), DebuggableEditorStyles.TextAreaStyle);
			}

			#region Render collections (arrays, lists, etc) - For now values are read-only!
			var fold = reflectedProperty.Fold;
			if (TryRenderEnumerableField(value, reflectedProperty.DisplayName, ref fold)) {
				reflectedProperty.Fold = fold;
				return value;
			}
			#endregion

			/*if (value != null && reflectedProperty is ReflectedPropertyWithDebuggableMembers reflectedPropertyWithDebuggableMembers) {
				reflectedPropertyWithDebuggableMembers.Fold = DevGUI.RenderCollapsibleBox(
					reflectedPropertyWithDebuggableMembers.Fold,
					reflectedPropertyWithDebuggableMembers.DisplayName,
					() => RenderDebuggableMembers(value, reflectedPropertyWithDebuggableMembers.DebuggableProperties, reflectedPropertyWithDebuggableMembers.DebuggableMethods),
					DevGUI.CollapsibleBoxStyle.FOLDOUT,
					true
				);
				return value;
			}*/

			return EditorExtensions.RenderField(reflectedProperty.Type, reflectedProperty.DisplayName, value, reflectedProperty.HelpText);
		}

		private static bool TryRenderEnumerableField(object value, string displayName, ref bool fold) {
			// string and Transform are special cases: they implement IEnumerable, but for the purposes of the editor we
			// don't want to treat them as such.
			if (value == null || !(value is IEnumerable enumerableValue) || value is string || value is Transform) {
				return false;
			}

			fold = EditorExtensions.ReadonlyEnumerable(fold, enumerableValue, displayName);
			return true;
		}
	}
}