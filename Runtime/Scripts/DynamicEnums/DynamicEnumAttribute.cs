using UnityEngine;

namespace CommonUtils.DynamicEnums {
	public class DynamicEnumAttribute : PropertyAttribute {
		public string EnumName;

		public DynamicEnumAttribute(string enumName) => EnumName = enumName;
	}
}