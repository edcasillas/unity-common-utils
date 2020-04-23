using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonUtils.Editor {
	public static class ReflectionUtils {
		#region GetMethodsWithAttribute
		public static IReadOnlyList<MethodAttributePair<TAttribute>> GetMethodsWithAttribute<TAttribute>(this object source) where TAttribute : Attribute => getMethodsWithAttribute<TAttribute>(source.GetType());

		public static IReadOnlyList<MethodAttributePair<TAttribute>> GetMethodsWithAttribute<T, TAttribute>() where TAttribute : Attribute => getMethodsWithAttribute<TAttribute>(typeof(T));

		private static IReadOnlyList<MethodAttributePair<TAttribute>> getMethodsWithAttribute<TAttribute>(Type type)
			where TAttribute : Attribute {
			var result = new List<MethodAttributePair<TAttribute>>();
			var allMethods = GetAllMethods(type);
			foreach (var methodInfo in allMethods) {
				foreach (TAttribute attribute in methodInfo.GetCustomAttributes(typeof(TAttribute), false)) {
					result.Add(new MethodAttributePair<TAttribute>{MethodInfo = methodInfo, Attribute = attribute});
				}
			}

			return result;
		}
		#endregion

		#region GetAllMethods
		public static IEnumerable<MethodInfo> GetAllMethods<T>() => GetAllMethods(typeof(T));

		public static IEnumerable<MethodInfo> GetAllMethods(Type t) {
			if (t == null)
				return Enumerable.Empty<MethodInfo>();
			var binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return t.GetMethods(binding).Concat(GetAllMethods(t.BaseType));
		}
		#endregion
	}

	public class MethodAttributePair<TAttribute> where TAttribute : Attribute {
		public MethodInfo MethodInfo;
		public TAttribute Attribute;
	}
}