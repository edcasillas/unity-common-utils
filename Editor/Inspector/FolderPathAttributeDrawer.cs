using System.IO;
using CommonUtils.Inspector;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public sealed class FolderPathAttributeDrawer : PropertyDrawer {
        private const float Spacing = 4f;
        private static readonly GUIContent FolderButtonContent = EditorGUIUtility.IconContent("Folder Icon");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.HelpBox(position, "FolderPath attribute can only be used on string fields.", MessageType.Error);
                return;
            }

            var folderAttribute = (FolderPathAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var buttonSize = position.height;
            var buttonRect = new Rect(position.xMax - buttonSize, position.y, buttonSize, position.height);
            var fieldRect = new Rect(position.x, position.y, position.width - buttonSize - Spacing, position.height);

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(!folderAttribute.AllowManualEdit);
            var typedValue = EditorGUI.TextField(fieldRect, property.stringValue);
            EditorGUI.EndDisabledGroup();
            var fieldChanged = EditorGUI.EndChangeCheck();
            if (folderAttribute.AllowManualEdit && fieldChanged) {
                property.stringValue = NormalizePath(typedValue, folderAttribute);
            }

            if (FolderButtonContent != null) {
                FolderButtonContent.tooltip = folderAttribute.Title;
            }

            if (GUI.Button(buttonRect, FolderButtonContent ?? GUIContent.none, EditorStyles.iconButton)) {
                var startingPath = ResolveStartingPath(property.stringValue, folderAttribute);
                if (string.IsNullOrEmpty(startingPath)) startingPath = folderAttribute.DefaultPath;
                var selected = EditorUtility.OpenFolderPanel(folderAttribute.Title, startingPath, string.Empty);
                if (!string.IsNullOrEmpty(selected)) {
                    property.stringValue = folderAttribute.IsRelativeToProject
                        ? FileUtil.GetProjectRelativePath(selected)
                        : selected;
                }
            }

            EditorGUI.EndProperty();
        }

        private static string NormalizePath(string value, FolderPathAttribute attribute) {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var trimmed = value.Trim();

            if (!attribute.IsRelativeToProject) return trimmed;

            var relative = FileUtil.GetProjectRelativePath(trimmed);
            return string.IsNullOrEmpty(relative) ? trimmed : relative;
        }

        private static string ResolveStartingPath(string storedValue, FolderPathAttribute attribute) {
            if (string.IsNullOrEmpty(storedValue)) return string.Empty;
            if (!attribute.IsRelativeToProject) return storedValue;

            if (!storedValue.StartsWith("Assets")) return storedValue;

            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var combined = Path.Combine(projectRoot, storedValue);
            return Directory.Exists(combined) ? combined : projectRoot;
        }
    }
}
