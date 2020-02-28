using CommonUtils.Inspector.HelpBox;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
    // Original credit: https://forum.unity.com/threads/helpattribute-allows-you-to-use-helpbox-in-the-unity-inspector-window.462768/#post-3014998
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer {

        public override float GetHeight() {
            if (!(attribute is HelpBoxAttribute helpBoxAttribute)) return base.GetHeight();
            var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
            if (helpBoxStyle == null) return base.GetHeight();
            return Mathf.Max(40f,
                             helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.text),
                                                     EditorGUIUtility.currentViewWidth) + 4);
        }

        public override void OnGUI(Rect position) {
            if (!(attribute is HelpBoxAttribute helpBoxAttribute)) return;
            EditorGUI.HelpBox(position, helpBoxAttribute.text, getMessageType(helpBoxAttribute.messageType));
        }

        private static MessageType getMessageType(HelpBoxMessageType helpBoxMessageType) {
            switch (helpBoxMessageType) {
                case HelpBoxMessageType.Info:    
                    return MessageType.Info;
                case HelpBoxMessageType.Warning: 
                    return MessageType.Warning;
                case HelpBoxMessageType.Error:   
                    return MessageType.Error;
                case HelpBoxMessageType.None:
                default:
                    return MessageType.None;
            }
        }
    }
}