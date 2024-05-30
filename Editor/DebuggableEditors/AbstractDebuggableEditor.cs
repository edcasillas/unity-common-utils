using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Editor.DebuggableEditors {
	public abstract class AbstractDebuggableEditor<T> : UnityEditor.Editor where T : Object {
		internal static readonly ComponentsCache cache = new(100);

		/// <summary>
		/// Gets a strongly typed reference of the <see cref="UnityEditor.Editor.target"/> of this custom editor.
		/// </summary>
		protected T Subject { get; set; }

		/// <summary>
		/// When set to <c>true</c>, the debuggable editor will render all public properties and methods, regardless of
		/// the attributes used.
		/// </summary>
		protected virtual bool DebugAllPropertiesAndMethods => false;

		internal DebuggableComponentData componentData;

		protected virtual void OnEnable() {
			if (!target) return;

			try {
				Subject = (T) target;
			}
			catch (InvalidCastException) {
				Debug.LogError($"Could not set the subject of the debuggable editor for the type '{typeof(T)}'. Will use the default inspector.", target);
			}

			componentData = InitializeComponentData();
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
			if (ShouldOnlyShowConfig()) {
				RenderConfig();
				return;
			}

			if (!componentData.HasDebuggableMembers()) {
				EditorGUILayout.HelpBox($"This component has a Debuggable Editor ({GetType().Name}) but no properties or methods have been tagged as Debuggable. Use the [ShowInInspector] attribute on properties and methods to expose them in this Editor.", MessageType.Info);
			}

			componentData.ShowConfig = EditorExtensions.Collapse(componentData.ShowConfig, "Configuration", RenderConfig);
			componentData.ShowDebug = EditorExtensions.Collapse(componentData.ShowDebug, "Debug", RenderDebug);
			EditorUtility.SetDirty(target);
		}

		internal abstract DebuggableComponentData InitializeComponentData();

		internal virtual bool ShouldOnlyShowConfig() => !Application.isPlaying;

		/// <summary>
		/// Inheritors can override this method to tweak the inspector for the <see cref="Subject"/>.
		/// When not overriden, calls <see cref="UnityEditor.Editor.DrawDefaultInspector"/>.
		/// </summary>
		protected virtual void RenderConfig() => DrawDefaultInspector();

		/// <summary>
		/// Inheritors must override this method to include any debug fields that might be useful in play mode.
		/// </summary>
		protected virtual void RenderDebug() => componentData.RenderDebuggableMembers(Subject);
	}
}