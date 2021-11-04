using CommonUtils.Input;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(SwipeManager))]
    public class SwipeManagerEditor : UnityEditor.Editor {
        private SwipeManager swipeManager;
        private bool showConfigAtRuntime;

        private void OnEnable() => swipeManager = (SwipeManager) target;

        public override void OnInspectorGUI() {
            if (Application.isPlaying) {
                EditorExtensions.BoxGroup(() => {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Toggle("Touch started", swipeManager.TouchStarted);
                    EditorGUILayout.Toggle("Touch ended", swipeManager.TouchEnded);
                    EditorGUILayout.Toggle("Swiping", swipeManager.Swiping);
                    EditorGUILayout.Toggle("SwipingHorizontal", swipeManager.SwipingHorizontal);
                    EditorGUILayout.Toggle("Swiping Vertical", swipeManager.SwipingVertical);
                    EditorGUILayout.Toggle("Swiping Left", swipeManager.SwipingLeft);
                    EditorGUILayout.Toggle("Swiping Right", swipeManager.SwipingRight);
                    EditorGUILayout.Toggle("Swiping Up", swipeManager.SwipingUp);
                    EditorGUILayout.Toggle("Swiping Down", swipeManager.SwipingDown);
                    EditorGUILayout.Toggle("Tapped", swipeManager.Tapped);
                    EditorGUI.EndDisabledGroup();
                }, "Debug");
                EditorUtility.SetDirty(target);

                showConfigAtRuntime =
                    EditorExtensions.Collapse(showConfigAtRuntime, "Config", () => DrawDefaultInspector());
            } else {
                DrawDefaultInspector();   
            }
        }
    }
}