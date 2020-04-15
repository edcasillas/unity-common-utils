using CommonUtils.DynamicEnums;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
	[CustomPropertyDrawer(typeof(DynamicEnumAttribute))]
	public class DynamicEnumDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if (property.propertyType != SerializedPropertyType.Integer && property.propertyType != SerializedPropertyType.String) {
				EditorGUI.HelpBox(position, "DynamicEnum value must be int or string.", MessageType.Error);
				return;
			}

			var dynamicEnumAtt = attribute as DynamicEnumAttribute;
			var enumValues = DynamicEnumManager.GetValuesAsGuiContent(dynamicEnumAtt.EnumName);

			if (enumValues == null) {
				EditorGUI.HelpBox(position, $"{dynamicEnumAtt.EnumName} is not defined as a DynamicEnum.", MessageType.Error);
				return;
			}

			var intValue = property.propertyType == SerializedPropertyType.Integer ? property.intValue : DynamicEnumManager.ValueToInt(dynamicEnumAtt.EnumName, property.stringValue);
			if (intValue < 0) intValue = 0;
			intValue = EditorGUI.Popup(position, label, intValue, enumValues);


			if (property.propertyType == SerializedPropertyType.Integer) {
				property.intValue = intValue;
			} else {
				property.stringValue = DynamicEnumManager.IntToValue(dynamicEnumAtt.EnumName, intValue);
			}
		}
	}
}