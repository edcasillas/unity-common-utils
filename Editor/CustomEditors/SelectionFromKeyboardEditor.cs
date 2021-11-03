using CommonUtils.Extensions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(SelectionFromKeyboard))]
    public class SelectionFromKeyboardEditor : UnityEditor.Editor {
        #region Constants
        private const string EditorPrefKey_ShowInSceneView = nameof(CommonUtils) + "." + nameof(SelectionFromKeyboardEditor) + "." + nameof(showInSceneView);
        private const string EditorPrefKey_RectSize = nameof(CommonUtils) + "." + nameof(SelectionFromKeyboardEditor) +"." + nameof(rectSize);
        private const string EditorPrefKey_RectFillColor = nameof(CommonUtils) + "." + nameof(SelectionFromKeyboardEditor) +"." + nameof(rectFillColor);
        private const string EditorPrefKey_RectOutlineColor = nameof(CommonUtils) + "." + nameof(SelectionFromKeyboardEditor) +"." + nameof(rectOutlineColor);
        private const string EditorPrefKey_RectOffSet = nameof(CommonUtils) + "." + nameof(SelectionFromKeyboardEditor) +"." + nameof(rectOffset);
        private const string EditorPrefKey_TextColor = nameof(CommonUtils) + "." + nameof(SelectionFromKeyboardEditor) +"." + nameof(textColor);
        #endregion
        
        #region Properties
        private static bool showInSceneView {
            get => EditorPrefs.GetBool(EditorPrefKey_ShowInSceneView, true);
            set => EditorPrefs.SetBool(EditorPrefKey_ShowInSceneView, value);
        }
        
        private static float rectSize {
            get => EditorPrefs.GetFloat(EditorPrefKey_RectSize, 25);
            set => EditorPrefs.SetFloat(EditorPrefKey_RectSize, value);
        }
        
        private static Color rectFillColor {
            get {
                var hex = EditorPrefs.GetString(EditorPrefKey_RectFillColor, Color.white.ColorToHex());
                return hex.HexToColor();
            }
            set => EditorPrefs.SetString(EditorPrefKey_RectFillColor, value.ColorToHex(true));
        }
        
        private static Color rectOutlineColor {
            get {
                var hex = EditorPrefs.GetString(EditorPrefKey_RectOutlineColor, Color.black.ColorToHex());
                return hex.HexToColor();
            }
            set => EditorPrefs.SetString(EditorPrefKey_RectOutlineColor, value.ColorToHex(true));
        }
        
        private static Vector2 rectOffset {
            get {
                var str = EditorPrefs.GetString(EditorPrefKey_RectOffSet, Vector2.zero.ToString());
                return str.ToVector2(new Vector2(0, 5));
            }
            set => EditorPrefs.SetString(EditorPrefKey_RectOffSet, value.ToString());
        }
        
        private static Color textColor {
            get {
                var hex = EditorPrefs.GetString(EditorPrefKey_TextColor, Color.black.ColorToHex());
                return hex.HexToColor();
            }
            set => EditorPrefs.SetString(EditorPrefKey_TextColor, value.ColorToHex(true));
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
                rectSize = EditorGUILayout.FloatField("Rect Size", rectSize);
                rectFillColor = EditorGUILayout.ColorField("Rect Fill Color", rectFillColor);
                rectOutlineColor = EditorGUILayout.ColorField("Rect Outline Color", rectOutlineColor);
                rectOffset = EditorGUILayout.Vector2Field("Rect Offset", rectOffset);
                textColor = EditorGUILayout.ColorField("Text Color", textColor);
            });
            DrawDefaultInspector();
        }

        private void OnSceneGUI() {
            if(!showInSceneView || Event.current.type != EventType.Repaint) return;
            var style = new GUIStyle {normal = {textColor = textColor}, alignment = TextAnchor.MiddleCenter};
            
            var halfSizeVector = Vector2.one * (rectSize / 2);
            var i = 0;
            foreach (var child in subject.childrenToSelect) {
                if (child.gameObject.activeInHierarchy) {
                    var bgRectCenter = (Vector2)child.transform.position - halfSizeVector - rectOffset;
                    Handles.DrawSolidRectangleWithOutline(new Rect(bgRectCenter, Vector2.one * rectSize), rectFillColor, rectOutlineColor);
                    Handles.Label(child.transform.position, (i++).ToString(), style);
                }
            }
        }
    }
}