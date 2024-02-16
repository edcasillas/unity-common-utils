using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CommonUtils.DebuggableEditors {
	public static class DebuggableEditorsUtils {
		#region Equality comparers
		private class PropertyInfoEqualityComparer : IEqualityComparer<PropertyInfo> {
			public bool Equals(PropertyInfo x, PropertyInfo y) => x.Name == y.Name && x.PropertyType == y.PropertyType;

			public int GetHashCode(PropertyInfo obj) {
				unchecked {
					int hash = 17;
					hash = hash * 23 + obj.Name.GetHashCode();
					hash = hash * 23 + obj.PropertyType.GetHashCode();
					return hash;
				}
			}
		}

		private class MethodInfoEqualityComparer : IEqualityComparer<MethodInfo> {
			public bool Equals(MethodInfo x, MethodInfo y) {
				// Check if both methods have the same signature
				return x.Name == y.Name &&
					   x.ReturnType == y.ReturnType &&
					   x.GetParameters()
						   .Select(p => p.ParameterType)
						   .SequenceEqual(y.GetParameters().Select(p => p.ParameterType));
			}

			public int GetHashCode(MethodInfo obj) {
				// Generate a hash code based on the method signature
				unchecked {
					int hash = 17;
					hash = hash * 23 + obj.Name.GetHashCode();
					hash = hash * 23 + obj.ReturnType.GetHashCode();
					foreach (var paramType in obj.GetParameters().Select(p => p.ParameterType)) {
						hash = hash * 23 + paramType.GetHashCode();
					}

					return hash;
				}
			}
		}
		#endregion

		private static readonly HashSet<PropertyInfo> monoBehaviourProperties = new HashSet<PropertyInfo>(typeof(MonoBehaviour).GetProperties(BindingFlags.Instance | BindingFlags.Public), new PropertyInfoEqualityComparer());
		private static readonly HashSet<MethodInfo> monoBehaviourMethods = new HashSet<MethodInfo>(typeof(MonoBehaviour).GetMethods(BindingFlags.Instance | BindingFlags.Public), new MethodInfoEqualityComparer());

		public static ICollection<ReflectedProperty> GetDebuggableProperties(this Type t, bool debugAllProperties = false, bool debugAllMonoBehaviorProperties = false) {
			var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(prop =>
				monoBehaviourProperties.Contains(prop) ?
					debugAllMonoBehaviorProperties && (prop.CanRead || prop.CanWrite) :
					debugAllProperties ?
						prop.CanRead || prop.CanWrite :
						Attribute.IsDefined(prop, typeof(ShowInInspectorAttribute)));

			var result = props.Select(p => {
				var showInInspectorAttribute = p.GetCustomAttribute<ShowInInspectorAttribute>();
				return new ReflectedProperty(p, showInInspectorAttribute?.DisplayName ?? p.Name.PascalToTitleCase()) {
					SetterIsEnabled = showInInspectorAttribute?.EnableSetter == true || p.GetSetMethod() != null,
					UseTextArea = showInInspectorAttribute?.UseTextArea == true,
					HelpText = $"From {p.DeclaringType}"
				};
			});
			return result.ToList();
		}

		public static ICollection<ReflectedMethod> GetDebuggableMethods(this Type t, bool debugAllMethods = false, bool debugAllMonoBehaviorMethods = false) {
			var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(method =>
				monoBehaviourMethods.Contains(method) ?
					debugAllMonoBehaviorMethods && method.IsPublic :
					debugAllMethods ?
						method.IsPublic && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_") :
						Attribute.IsDefined(method, typeof(ShowInInspectorAttribute)));

			var result = methods.Select(m => {
				var showInInspectorAttribute = m.GetCustomAttribute<ShowInInspectorAttribute>();
				return new ReflectedMethod(m, showInInspectorAttribute?.DisplayName ?? m.Name.PascalToTitleCase());
			});
			return result.ToList();
		}
	}
}