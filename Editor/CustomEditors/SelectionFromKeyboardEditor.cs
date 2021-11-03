using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.CustomEditors {
    [CustomEditor(typeof(SelectionFromKeyboard))]
    public class SelectionFromKeyboardEditor : UnityEditor.Editor {
        private SelectionFromKeyboard subject;

        private void OnEnable() {
            subject = (SelectionFromKeyboard) target;
        }

        private void OnSceneGUI() {
            if(Event.current.type != EventType.Repaint) return;
            var style = new GUIStyle {normal = {textColor = Color.black}, alignment = TextAnchor.MiddleCenter};

            const float rectSize = 25;
            var halfSizeVector = Vector2.one * (rectSize / 2);
            var offset = new Vector2(0, 5);
            var i = 0;
            foreach (var child in subject.childrenToSelect) {
                if (child.gameObject.activeInHierarchy) {
                    var bgRectCenter = (Vector2)child.transform.position - halfSizeVector - offset;
                    Handles.DrawSolidRectangleWithOutline(new Rect(bgRectCenter, Vector2.one * rectSize), Color.white, Color.black);
                    Handles.Label(child.transform.position, (i++).ToString(), style);
                }
            }
        }
    }
}