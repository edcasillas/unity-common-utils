using System;
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
		private static readonly ComponentsCache cache = new ComponentsCache(100);

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

		private DebuggableComponentData componentData;

		protected virtual void OnEnable() {
			if (!target) return;

            try {
                Subject = (T) target;
            }
            catch (InvalidCastException) {
                Debug.LogError($"Could not set the subject of the debuggable editor for the type '{typeof(T)}'. Will use the default inspector.", target);
            }

			componentData = cache.Get(Subject, DebugAllPropertiesAndMethods, DebugAllMonoBehaviorPropertiesAndMethods);
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

			if (!componentData.HasDebuggableMembers()) {
				EditorGUILayout.HelpBox($"This component has a Debuggable Editor ({GetType().Name}) but no properties or methods have been tagged as Debuggable. Use the [ShowInInspector] attribute on properties and methods to expose them in this Editor.", MessageType.Info);
			}

            componentData.ShowConfig = EditorExtensions.Collapse(componentData.ShowConfig, "Configuration", RenderConfig);
            componentData.ShowDebug = EditorExtensions.Collapse(componentData.ShowDebug, "Debug", RenderDebug);
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
		protected virtual void RenderDebug() => componentData.RenderDebuggableMembers(Subject);
    }
}