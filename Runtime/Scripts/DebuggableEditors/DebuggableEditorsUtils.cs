using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CommonUtils.DebuggableEditors {
	public static class DebuggableEditorsUtils {
		private static readonly PropertyInfo[] monoBehaviourProperties = typeof(MonoBehaviour).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
		private static readonly MethodInfo[] monoBehaviourMethods = typeof(MonoBehaviour).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

		public static ICollection<ReflectedProperty> GetDebuggableProperties(this Type t, bool debugAllProperties = false, bool debugAllMonoBehaviorProperties = false) {
			var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(prop => (debugAllProperties && (prop.GetGetMethod()?.IsPublic == true || prop.GetSetMethod()?.IsPublic == true)) || Attribute.IsDefined(prop, typeof(ShowInInspectorAttribute)));

			if (debugAllProperties && !debugAllMonoBehaviorProperties) props = props.Except(monoBehaviourProperties);
			if (!debugAllProperties && debugAllMonoBehaviorProperties) props = props.Concat(monoBehaviourProperties);

			var result = props.Select(p => {
				var showInInspectorAttribute = p.GetCustomAttribute<ShowInInspectorAttribute>();
				return new ReflectedProperty(p, showInInspectorAttribute?.DisplayName ?? p.Name) {
					SetterIsEnabled = showInInspectorAttribute?.EnableSetter == true || p.GetSetMethod() != null,
					UseTextArea = showInInspectorAttribute?.UseTextArea == true,
				};
			});
			return result.ToList();
		}

		public static ICollection<ReflectedMethod> GetDebuggableMethods(this Type t, bool debugAllMethods = false, bool debugAllMonoBehaviorMethods = false) {
			var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(method => (debugAllMethods && method.IsPublic) || Attribute.IsDefined(method, typeof(ShowInInspectorAttribute)));
			if (debugAllMethods && !debugAllMonoBehaviorMethods) methods = methods.Except(monoBehaviourMethods);
			if (!debugAllMethods && debugAllMonoBehaviorMethods) methods = methods.Concat(monoBehaviourMethods);

			var result = methods.Select(m => {
				var showInInspectorAttribute = m.GetCustomAttribute<ShowInInspectorAttribute>();
				return new ReflectedMethod(m, showInInspectorAttribute?.DisplayName ?? m.Name);
			});
			return result.ToList();
		}
	}
}