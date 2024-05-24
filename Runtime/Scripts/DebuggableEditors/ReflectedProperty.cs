using System;
using System.Reflection;
using UnityEngine;

namespace CommonUtils.DebuggableEditors {
	public class ReflectedProperty : AbstractReflectedMember<PropertyInfo>  {
		public bool HasPublicGetter { get; }
		public bool HasPublicSetter { get; }
		public bool SetterIsEnabled { get; set; }
		public bool UseTextArea { get; set; }
		public string HelpText { get; set; }
		public override Type Type => MemberInfo.PropertyType;

		public ReflectedProperty(PropertyInfo propertyInfo, string displayName) : base(propertyInfo, displayName) {
			HasPublicGetter = propertyInfo.CanRead;
			HasPublicSetter = propertyInfo.CanWrite;
		}

		public object GetValue(object instance) => MemberInfo.GetValue(instance);

		public void SetValue(object instance, object value) {
			try {
				MemberInfo.SetValue(instance, value);
			} catch (Exception ex) {
				Debug.LogError($"{ex.GetType().Name} occurred while setting the value of property \"{DisplayName}\": {ex.Message}");
			}
		}
	}
}