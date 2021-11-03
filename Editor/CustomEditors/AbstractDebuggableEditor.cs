using System;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
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

        private bool showConfig = false;
        private bool showDebug = false;

        private void OnEnable() {
            if (!target) return;

            try {
                Subject = (T) target;
            }
            catch (InvalidCastException) {
                Debug.LogError($"Could not set the subject of the debuggable editor for the type '{typeof(T)}'. Will use the default inspector.", target);
            }
        }

        public override void OnInspectorGUI() {
            if (!Subject) {
                DrawDefaultInspector();
                return;
            }

            if (!Application.isPlaying) {
                RenderConfig();
                return;
            }

            showConfig = EditorExtensions.Collapse(showConfig, "Configuration", RenderConfig);
            showDebug = EditorExtensions.Collapse(showDebug, "Debug", RenderDebug);
        }

        /// <summary>
        /// Inheritors can override this method to tweak the inspector for the <see cref="Subject"/>.
        /// When not overriden, calls <see cref="UnityEditor.Editor.DrawDefaultInspector"/>.
        /// </summary>
        protected virtual void RenderConfig() => DrawDefaultInspector();

        
        /// <summary>
        /// Inheritors must override this method to include any debug fields that might be useful in play mode.
        /// </summary>
        protected abstract void RenderDebug();
    }
}