using CommonUtils.Extensions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(SelectionFromKeyboard))]
    public class SelectionFromKeyboardEditor : UnityEditor.Editor {
        #region Constants
        private const string EditorPrefKey_ShowInSceneView = nameof(CommonUtils) + "." + nameof(SelectionFromKeyboardEditor) + "." + nameof(showInSceneView);
        private const string EditorPrefKey_RectFillColor = nameof(CommonUtils) + "." + nameof(SelectionFromKeyboardEditor) +"." + nameof(rectFillColor);
        #endregion
        
        #region Properties
        private static bool showInSceneView {
            get => EditorPrefs.GetBool(EditorPrefKey_ShowInSceneView, true);
            set => EditorPrefs.SetBool(EditorPrefKey_ShowInSceneView, value);
        }
        
        private static Color rectFillColor {
            get {
                var hex = EditorPrefs.GetString(EditorPrefKey_RectFillColor, Color.white.ColorToHex());
                return hex.HexToColor();
            }
            set => EditorPrefs.SetString(EditorPrefKey_RectFillColor, value.ColorToHex(true));
        }

        #endregion
        
        private SelectionFromKeyboard subject;
        private bool showDisplayOptions;

        private void OnEnable() {
            subject = (SelectionFromKeyboard) target;
        }

        public override void OnInspectorGUI() {
            showDisplayOptions = EditorExtensions.Collapse(showDisplayOptions, "Display Options", () => {
                showInSceneView = EditorGUILayout.Toggle("Show Children Indices in Scene View", showInSceneView);
                rectFillColor = EditorGUILayout.ColorField("Rect Fill Color", rectFillColor);
            });
            DrawDefaultInspector();
        }

        private void OnSceneGUI() {
            if(!showInSceneView || Event.current.type != EventType.Repaint) return;
            var style = new GUIStyle {normal = {textColor = Color.black}, alignment = TextAnchor.MiddleCenter};

            const float rectSize = 25;
            var halfSizeVector = Vector2.one * (rectSize / 2);
            var offset = new Vector2(0, 5);
            var i = 0;
            foreach (var child in subject.childrenToSelect) {
                if (child.gameObject.activeInHierarchy) {
                    var bgRectCenter = (Vector2)child.transform.position - halfSizeVector - offset;
                    Handles.DrawSolidRectangleWithOutline(new Rect(bgRectCenter, Vector2.one * rectSize), rectFillColor, Color.black);
                    Handles.Label(child.transform.position, (i++).ToString(), style);
                }
            }
        }
    }
}