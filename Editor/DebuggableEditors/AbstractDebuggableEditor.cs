using CommonUtils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.DebuggableEditors {
	/// <summary>
    /// Base class for custom editors that show two sections while playing in the editor:
    /// - "Configuration" shows the default inspector to set up the target <see cref="Subject"/>,
    /// or whatever modifications are done to this default inspector by overriding <see cref="RenderConfig"/>.
    /// - "Debug" shows values that only make sense during play mode. Inheritors must override <see cref="RenderDebug"/>.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="MonoBehaviour"/> for which this class will be a custom editor.</typeparam>
    public abstract class AbstractDebuggableEditor<T> : UnityEditor.Editor where T : MonoBehaviour {
		/// <summary>
        /// Gets a strongly typed reference of the <see cref="UnityEditor.Editor.target"/> of this custom editor.
        /// </summary>
        protected T Subject { get; private set; }

		/// <summary>
		/// When set to <c>true</c>, the debuggable editor will render all public properties and methods, regardless of
		/// the attributes used.
		/// </summary>
		protected virtual bool DebugAllPropertiesAndMethods => false;

		/// <summary>
		/// When set to <c>true</c> and <see cref="DebugAllPropertiesAndMethods"/> is also <c>true</c>, the debuggable
		/// editor will include public properties and methods from <see cref="MonoBehaviour"/> and base classes.
		/// </summary>
		protected virtual bool DebugAllMonoBehaviorPropertiesAndMethods => false;

		private ICollection<ReflectedProperty> debuggableProperties;
		private ICollection<ReflectedMethod> debuggableMethods;
        private bool showConfig;
        private bool showDebug = true;

        protected virtual void OnEnable() {
            if (!target) return;

            try {
                Subject = (T) target;
            }
            catch (InvalidCastException) {
                Debug.LogError($"Could not set the subject of the debuggable editor for the type '{typeof(T)}'. Will use the default inspector.", target);
            }

			debuggableProperties = target.GetType().GetDebuggableProperties(DebugAllPropertiesAndMethods, DebugAllMonoBehaviorPropertiesAndMethods);
			debuggableMethods = target.GetType().GetDebuggableMethods(DebugAllPropertiesAndMethods, DebugAllMonoBehaviorPropertiesAndMethods);
		}

        public override void OnInspectorGUI() {
            if (!Subject) {
                DrawDefaultInspector();
                return;
            }

			EditorGUILayout.Space();

			/*
			 * When not in play mode, show only the config section.
			 * Also use this behavior when the inspected object is a prefab, i.e. is not in a scene.
			 */
            if (!Application.isPlaying || Subject.gameObject.scene.rootCount == 0) {
                RenderConfig();
                return;
            }

			if (!debuggableProperties.Any() && !debuggableMethods.Any()) {
				EditorGUILayout.HelpBox($"This component has a Debuggable Editor ({GetType().Name}) but no properties or methods have been tagged as Debuggable. Use the [ShowInInspector] attribute on properties and methods to expose them in this Editor.", MessageType.Info);
			}

            showConfig = EditorExtensions.Collapse(showConfig, "Configuration", RenderConfig);
            showDebug = EditorExtensions.Collapse(showDebug, "Debug", RenderDebug);
			EditorUtility.SetDirty(target);
        }

        /// <summary>
        /// Inheritors can override this method to tweak the inspector for the <see cref="Subject"/>.
        /// When not overriden, calls <see cref="UnityEditor.Editor.DrawDefaultInspector"/>.
        /// </summary>
        protected virtual void RenderConfig() => DrawDefaultInspector();

		/// <summary>
		/// Inheritors must override this method to include any debug fields that might be useful in play mode.
		/// </summary>
		protected virtual void RenderDebug() => renderDebuggableMembers(Subject, debuggableProperties, debuggableMethods);

		private void renderDebuggableMembers(object instance, ICollection<ReflectedProperty> properties, ICollection<ReflectedMethod> methods) {
			foreach (var prop in properties) {
				renderProperty(instance, prop);
			}

			foreach (var method in methods) {
				renderMethod(instance, method);
			}
		}

		private void renderProperty(object instance, ReflectedProperty reflectedProperty) {
			if (!reflectedProperty.HasPublicGetter) {
				EditorGUILayout.HelpBox(
					$"Property {reflectedProperty.RealName} has been tagged with {nameof(ShowInInspectorAttribute)} but the value cannot be read because it doesn't have a setter.",
					MessageType.Error
				);
				return;
			}

			if (reflectedProperty.SetterIsEnabled) {
				if (!reflectedProperty.HasPublicSetter) {
					EditorGUILayout.HelpBox(
						$"Property {reflectedProperty.RealName} has been tagged with {nameof(ShowInInspectorAttribute)} with EnableSetter "
					  + $"= true, but a public setter has not been defined, the property is therefore read-only.",
						MessageType.Error
					);
				}
			}

			object oldValue;
			try {
				oldValue = reflectedProperty.GetValue(instance);
			} catch (Exception ex) {
				EditorGUILayout.HelpBox($"An exception occurred while calling the setter of property \"{reflectedProperty.RealName}\": {ex.Message}", MessageType.Error);
				return;
			}

			reflectedProperty.RenderDebugInfoIfAny();

			var newValue = renderEditorField(
				reflectedProperty,
				oldValue
			);

			if (reflectedProperty.SetterIsEnabled && reflectedProperty.HasPublicSetter && !oldValue.Equals(newValue)) {
				reflectedProperty.SetValue(instance, newValue);
			}
		}

		private void renderMethod(object instance, ReflectedMethod reflectedMethod) {
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
				for (int i = 0; i < reflectedMethod.ParamInfo.Length; i++) {
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

			if (reflectedMethod.HasBeenCalled && reflectedMethod.HasReturnValue) {
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

			GUILayout.EndVertical();
		}

		private object renderEditorField(ReflectedProperty reflectedProperty, object value) {
			if (reflectedProperty.UseTextArea) {
				EditorGUILayout.LabelField(reflectedProperty.DisplayName);
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

			return EditorExtensions.RenderField(reflectedProperty.Type, reflectedProperty.DisplayName, value);
		}

		private bool TryRenderEnumerableField(object value, string displayName, ref bool fold) {
			// string and Transform are special cases: they implement IEnumerable, but for the purposes of the editor we
			// don't want to treat them as such.
			if (value == null || !(value is IEnumerable enumerableValue) || value is string || value is Transform) {
				return false;
			}

			var count = enumerableValue.Cast<object>().Count();

			if (count == 0) {
				EditorGUILayout.LabelField($"{displayName} is empty.");
			} else {
				fold = EditorGUILayout.Foldout(fold, new GUIContent($"{displayName}"), true);
				if (!fold) return true;

				EditorGUI.indentLevel++;

				var i = 0;
				foreach (var item in enumerableValue) {
					if (item == null) {
						EditorGUILayout.LabelField($"[{i}]", "<null>");
					} else {
						EditorExtensions.RenderField(item.GetType(), $"[{i}]", item);
					}
					i++;
				}

				EditorGUI.indentLevel--;
			}

			return true;
		}
    }
}