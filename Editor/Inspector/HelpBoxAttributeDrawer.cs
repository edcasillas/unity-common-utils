using CommonUtils.Inspector.HelpBox;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
    // Original credit: https://forum.unity.com/threads/helpattribute-allows-you-to-use-helpbox-in-the-unity-inspector-window.462768/#post-3014998
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : DecoratorDrawer {
        private const float MinHeight = 40f;
        private const float LineHeight = 18f;
        private const float VerticalPadding = 12f;
        private const int ApproximateCharactersPerLine = 58;

        public override float GetHeight() {
            if (!(attribute is HelpBoxAttribute helpBoxAttribute)) return base.GetHeight();
            return Mathf.Max(MinHeight, getEstimatedLineCount(helpBoxAttribute.text) * LineHeight + VerticalPadding);
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

        private static int getEstimatedLineCount(string text) {
            if (string.IsNullOrEmpty(text)) return 1;

            var lineCount = 1;
            var currentLineLength = 0;

            foreach (var character in text) {
                if (character == '\n') {
                    lineCount++;
                    currentLineLength = 0;
                    continue;
                }

                currentLineLength++;

                if (currentLineLength < ApproximateCharactersPerLine) continue;

                lineCount++;
                currentLineLength = 0;
            }

            return lineCount;
        }
    }
}
