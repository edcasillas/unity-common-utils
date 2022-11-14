using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonUtils.Editor.DebuggableEditors {
	public static class DebuggableEditorsUtils {
		// TODO Finish writing these methods
		public static ICollection<ReflectedProperty> GetDebuggableProperties(this Type t, bool debugAllPropsAndMethods = false, bool debugAllMonoBehaviorPropsAndMethods = false) {
			var props = t.GetProperties().Where(
				prop => Attribute.IsDefined(prop, typeof(ShowInInspectorAttribute)));
			var p2 = props.Select(p => {
				var showInInspectorAttribute = p.GetCustomAttribute<ShowInInspectorAttribute>();
				return new ReflectedProperty(p, showInInspectorAttribute.DisplayName) {
					SetterIsEnabled = showInInspectorAttribute.EnableSetter,
					UseTextArea = showInInspectorAttribute.UseTextArea,
				};
			});
			return p2.ToList();
		}

		public static ICollection<ReflectedMethod> GetDebuggableMethods(this Type t, bool debugAllPropsAndMethods = false, bool debugAllMonoBehaviorPropsAndMethods = false) {
			var methods = t.GetMethods().Where(
				prop => Attribute.IsDefined(prop, typeof(ShowInInspectorAttribute)));
			var p2 = methods.Select(m => {
				var showInInspectorAttribute = m.GetCustomAttribute<ShowInInspectorAttribute>();
				return new ReflectedMethod(m, showInInspectorAttribute.DisplayName) {

				};
			});
			return p2.ToList();
		}
	}
}