using UnityEditor;

namespace SubjectNerd.Utilities {
	public static class ReorderableArrayUtils {
		public static string GetGrandParentPath(SerializedProperty property) {
			string parent = property.propertyPath;
			int firstDot = property.propertyPath.IndexOf('.');
			if (firstDot > 0) {
				parent = property.propertyPath.Substring(0, firstDot);
			}

			return parent;
		}
	}
}