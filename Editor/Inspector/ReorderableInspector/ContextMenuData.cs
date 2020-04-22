using System.Collections.Generic;
using System.Reflection;
using CommonUtils.Editor;
using UnityEngine;

namespace SubjectNerd.Utilities {
	public struct ContextMenuData {
		public string menuItem;
		public MethodInfo function;
		public MethodInfo validate;

		public ContextMenuData(string item) {
			menuItem = item;
			function = null;
			validate = null;
		}
	}

	public static class ContextMenuDataExtensions {
		public static void GetContextMenuData(this Object target, Dictionary<string, ContextMenuData> contextData) {
			contextData.Clear();

			var contextMenuMethods = target.GetMethodsWithAttribute<ContextMenu>();
			foreach (var method in contextMenuMethods) {
				if (contextData.ContainsKey(method.Attribute.menuItem))
				{
					var data = contextData[method.Attribute.menuItem];
					if (method.Attribute.validate)
						data.validate = method.MethodInfo;
					else
						data.function = method.MethodInfo;
					contextData[data.menuItem] = data;
				}
				else
				{
					var data = new ContextMenuData(method.Attribute.menuItem);
					if (method.Attribute.validate)
						data.validate = method.MethodInfo;
					else
						data.function = method.MethodInfo;
					contextData.Add(data.menuItem, data);
				}
			}
		}
	}
}